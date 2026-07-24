using AutoMapper;
using BankingSystem.Constants;
using BankingSystem.Data.models;
using BankingSystem.DTOS.Shared;
using BankingSystem.DTOS.Transactions_TransfersDomain;
using BankingSystem.DTOS.Transactions_TransfersDomain.Request_DTOs;
using BankingSystem.DTOS.Transactions_TransfersDomain.Response_DTOs;
using BankingSystem.Exceptions;
using BankingSystem.Repository.GenericRepository.UnitOfWork;
using Microsoft.EntityFrameworkCore;

namespace BankingSystem.Services.TransactionService
{
    public class TransactionService : ITransactionService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<TransactionService> _logger;
        private readonly IMapper _mapper;

        public TransactionService(
            ILogger<TransactionService> logger,
            IMapper mapper,
            IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
            _mapper = mapper;
        }

        public async Task<ApiResponseDto<TransactionResponseDto>> TransferFundsAsync(TransferFundsDto requestDto, string userId)
        {
            if (requestDto.Amount <= 0)
            {
                throw new BadRequestException("Transfer amount must be greater than zero.");
            }

            if (requestDto.FromAccountNumber == requestDto.ToAccountNumber)
            {
                throw new BadRequestException("Cannot transfer funds to the same account.");
            }

            var customer = await _unitOfWork.Customers.GetCustomerByUserIdAsync(userId, false);
            if (customer == null || !customer.IsActive)
            {
                throw new UnauthorizedException("User profile not found or inactive.");
            }

            await using var transaction = await _unitOfWork.BeginTransactionAsync();

            try
            {
                var sourceAccount = await _unitOfWork.Accounts.GetAccountByNumberAsync(requestDto.FromAccountNumber, trackChanges: true);
                var destinationAccount = await _unitOfWork.Accounts.GetAccountByNumberAsync(requestDto.ToAccountNumber, trackChanges: true);

                if (sourceAccount == null || !sourceAccount.IsActive)
                {
                    throw new NotFoundException("Source account not found or inactive.");
                }

                if (destinationAccount == null || !destinationAccount.IsActive)
                {
                    throw new NotFoundException("Destination account not found or inactive.");
                }

                if (sourceAccount.CustomerId != customer.Id)
                {
                    _logger.LogWarning("Security Warning: User {UserId} attempted to transfer from unauthorized Account {AccountNumber}.", userId, requestDto.FromAccountNumber);
                    throw new UnauthorizedException("You are not authorized to transfer from this account.");
                }

                if (sourceAccount.Balance < requestDto.Amount)
                {
                    throw new BadRequestException("Insufficient funds in the source account.");
                }

                sourceAccount.Balance -= requestDto.Amount;
                destinationAccount.Balance += requestDto.Amount;

                var withdrawTransaction = new Transaction
                {
                    Amount = requestDto.Amount,
                    Type = TransactionType.Withdraw,
                    TransactionDate = DateTime.UtcNow,
                    AccountNumber = sourceAccount.AccountNumber
                };

                var depositTransaction = new Transaction
                {
                    Amount = requestDto.Amount,
                    Type = TransactionType.Deposit,
                    TransactionDate = DateTime.UtcNow,
                    AccountNumber = destinationAccount.AccountNumber
                };

                await _unitOfWork.Transactions.AddAsync(withdrawTransaction);
                await _unitOfWork.Transactions.AddAsync(depositTransaction);

                await _unitOfWork.SaveChangesAsync();

                await transaction.CommitAsync();

                _logger.LogInformation("Transfer of {Amount} from Account {Source} to Account {Destination} completed successfully by User {UserId}.",
                    requestDto.Amount, sourceAccount.AccountNumber, destinationAccount.AccountNumber, userId);

                return new ApiResponseDto<TransactionResponseDto>
                {
                    Message = "Transfer completed successfully.",
                    Data = _mapper.Map<TransactionResponseDto>(withdrawTransaction)
                };
            }
            catch (DbUpdateConcurrencyException ex)
            {
                await transaction.RollbackAsync();
                _logger.LogWarning(ex, "Concurrency conflict occurred during transfer from Account {FromAccountNumber} by User {UserId}.", requestDto.FromAccountNumber, userId);
                throw new ConflictException("The account balance was updated by another transaction. Please try again.");
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                _logger.LogError(ex, "An error occurred during transfer from Account {FromAccountNumber} to Account {Destination}.", requestDto.FromAccountNumber, requestDto.ToAccountNumber);
                throw;
            }
        }

        public async Task<ApiResponseDto<TransactionResponseDto>> DepositAsync(DepositWithdrawDto requestDto, string userId)
        {
            if (requestDto.Amount <= 0)
            {
                throw new BadRequestException("Deposit amount must be greater than zero.");
            }

            var customer = await _unitOfWork.Customers.GetCustomerByUserIdAsync(userId, false);
            if (customer == null)
            {
                throw new UnauthorizedException("User profile not found.");
            }

            var account = await _unitOfWork.Accounts.GetAccountByNumberAsync(requestDto.AccountNumber, trackChanges: true);

            if (account == null || !account.IsActive)
            {
                throw new NotFoundException("Account not found or inactive.");
            }

            if (account.CustomerId != customer.Id)
            {
                throw new UnauthorizedException("You are not authorized to deposit into this account.");
            }
            await using var transaction = await _unitOfWork.BeginTransactionAsync();
            try
            {
                account.Balance += requestDto.Amount;

                var depositTransaction = new Transaction
                {
                    Amount = requestDto.Amount,
                    Type = TransactionType.Deposit,
                    TransactionDate = DateTime.UtcNow,
                    AccountNumber = account.AccountNumber
                };

                await _unitOfWork.Transactions.AddAsync(depositTransaction);
                await _unitOfWork.SaveChangesAsync();

                _logger.LogInformation("Deposit of {Amount} to Account {AccountNumber} completed successfully by User {UserId}.", requestDto.Amount, account.AccountNumber, userId);

                await transaction.CommitAsync();
                return new ApiResponseDto<TransactionResponseDto>
                {
                    Message = "Deposit successful.",
                    Data = _mapper.Map<TransactionResponseDto>(depositTransaction)
                };
            }
            catch (DbUpdateConcurrencyException ex)
            {
                await transaction.RollbackAsync();
                _logger.LogWarning(ex, "Concurrency conflict occurred during deposit to Account {AccountNumber}.", requestDto.AccountNumber);
                throw new ConflictException("The account balance was updated by another process. Please try again.");
            }
        }

        public async Task<ApiResponseDto<PaginatedResponseDto<TransactionResponseDto>>> GetAccountStatementAsync(int accountNumber, PaginationRequestDto paginationDto, string userId)
        {
            var customer = await _unitOfWork.Customers.GetCustomerByUserIdAsync(userId, false);
            if (customer == null)
            {
                throw new UnauthorizedException("User profile not found.");
            }

            var account = await _unitOfWork.Accounts.GetAccountByNumberAsync(accountNumber, trackChanges: false);

            if (account == null)
            {
                throw new NotFoundException("Account not found.");
            }

            if (account.CustomerId != customer.Id)
            {
                _logger.LogWarning("Security Warning: User {UserId} attempted to view statement for unauthorized Account {AccountNumber}.", userId, accountNumber);
                throw new UnauthorizedException("You are not authorized to view this account's statement.");
            }

            var (transactions, totalCount) = await _unitOfWork.Transactions.GetTransactionsByAccountIdPaginatedAsync(
                accountNumber,
                paginationDto.PageNumber,
                paginationDto.PageSize,
                trackChanges: false
            );

            int totalPages = (int)Math.Ceiling(totalCount / (double)paginationDto.PageSize);
            var mappedTransactions = _mapper.Map<List<TransactionResponseDto>>(transactions);

            return new ApiResponseDto<PaginatedResponseDto<TransactionResponseDto>>
            {
                Message = "Account statement retrieved successfully.",
                Data = new PaginatedResponseDto<TransactionResponseDto>
                {
                    CurrentPage = paginationDto.PageNumber,
                    PageSize = paginationDto.PageSize,
                    TotalCount = totalCount,
                    TotalPages = totalPages,
                    Data = mappedTransactions
                }
            };
        }
    }
}