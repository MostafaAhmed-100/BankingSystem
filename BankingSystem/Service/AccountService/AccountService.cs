using AutoMapper;
using BankingSystem.Constants;
using BankingSystem.Data.models;
using BankingSystem.DTOS.Account_DTOs.Request_DTOs;
using BankingSystem.DTOS.Account_DTOs.ResponseDto;
using BankingSystem.DTOS.Shared;
using BankingSystem.Exceptions;
using BankingSystem.Repository.GenericRepository.UnitOfWork;

namespace BankingSystem.Services.AccountService
{
    public class AccountService : IAccountService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<AccountService> _logger;
        private readonly IMapper _mapper;

        public AccountService(
            ILogger<AccountService> logger,
            IMapper mapper,
            IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
            _mapper = mapper;
        }

        public async Task<ApiResponseDto<AccountResponseDto>> CreateAccountAsync(CreateAccountDto requestDto, string userId)
        {
            var customer = await _unitOfWork.Customers.GetCustomerByUserIdAsync(userId,false);
            if (customer == null || !customer.IsActive)
            {
                _logger.LogWarning("Account creation failed: Customer profile not found or inactive for UserId {UserId}.", userId);
                throw new NotFoundException("Customer profile not found or inactive.");
            }

            if (requestDto.InitialDeposit < 0)
            {
                throw new BadRequestException("Initial deposit cannot be negative.");
            }

            var account = new Account
            {
                Balance = requestDto.InitialDeposit,
                CurrencyCode = requestDto.CurrencyCode,
                CustomerId = customer.Id,
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            };

            if (requestDto.InitialDeposit > 0)
            {
                account.Transactions.Add(new Transaction
                {
                    Amount = requestDto.InitialDeposit,
                    Type = TransactionType.Deposit,
                    TransactionDate = DateTime.UtcNow
                });
            }

            await _unitOfWork.Accounts.AddAsync(account);
            await _unitOfWork.SaveChangesAsync();

            _logger.LogInformation("Account {AccountNumber} created successfully for Customer {CustomerId}.", account.AccountNumber, customer.Id);

            return new ApiResponseDto<AccountResponseDto>
            {
                Message = "Account created successfully.",
                Data = _mapper.Map<AccountResponseDto>(account)
            };
        }

        public async Task<ApiResponseDto<AccountResponseDto>> GetAccountBalanceAsync(int accountNumber, string userId)
        {
            var customer = await _unitOfWork.Customers.GetCustomerByUserIdAsync(userId,false);
            if (customer == null)
            {
                throw new UnauthorizedException("User profile not found.");
            }

            var account = await _unitOfWork.Accounts.GetAccountByNumberAsync(accountNumber, trackChanges: false);

            if (account == null || !account.IsActive)
            {
                _logger.LogWarning("Balance check failed: Account {AccountNumber} not found or inactive.", accountNumber);
                throw new NotFoundException("Account not found or inactive.");
            }

            if (account.CustomerId != customer.Id)
            {
                _logger.LogWarning("Security Warning: User {UserId} attempted to view balance for unauthorized Account {AccountNumber}.", userId, accountNumber);
                throw new UnauthorizedException("You are not authorized to view this account's balance.");
            }

            return new ApiResponseDto<AccountResponseDto>
            {
                Message = "Balance retrieved successfully.",
                Data = new AccountResponseDto
                {
                    AccountNumber = account.AccountNumber,
                    Balance = account.Balance,
                    CurrencyCode = account.CurrencyCode,
                }
            };
        }

        public async Task<ApiResponseDto<PaginatedResponseDto<AccountResponseDto>>> GetCustomerAccountsAsync(string userId, PaginationRequestDto paginationDto)
        {
            var customer = await _unitOfWork.Customers.GetCustomerByUserIdAsync(userId,false);
            if (customer == null)
            {
                throw new NotFoundException("Customer profile not found.");
            }

            var (accounts, totalCount) = await _unitOfWork.Accounts.GetAccountsByCustomerIdPaginatedAsync(
                customer.Id,
                paginationDto.PageNumber,
                paginationDto.PageSize,
                trackChanges: false
            );

            int totalPages = (int)Math.Ceiling(totalCount / (double)paginationDto.PageSize);
            var mappedAccounts = _mapper.Map<List<AccountResponseDto>>(accounts);

            return new ApiResponseDto<PaginatedResponseDto<AccountResponseDto>>
            {
                Message = "Accounts retrieved successfully.",
                Data = new PaginatedResponseDto<AccountResponseDto>
                {
                    CurrentPage = paginationDto.PageNumber,
                    PageSize = paginationDto.PageSize,
                    TotalCount = totalCount,
                    TotalPages = totalPages,
                    Data = mappedAccounts
                }
            };
        }

        public async Task<ApiResponseDto<string>> ToggleAccountStatusAsync(int accountNumber, string userId)
        {
            var customer = await _unitOfWork.Customers.GetCustomerByUserIdAsync(userId,false);
            if (customer == null)
            {
                throw new UnauthorizedException("User profile not found.");
            }

            var account = await _unitOfWork.Accounts.GetAccountByNumberAsync(accountNumber, trackChanges: true);

            if (account == null)
            {
                throw new NotFoundException("Account not found.");
            }

            if (account.CustomerId != customer.Id)
            {
                _logger.LogWarning("Security Warning: User {UserId} attempted to toggle status for unauthorized Account {AccountNumber}.", userId, accountNumber);
                throw new UnauthorizedException("You are not authorized to modify this account.");
            }

            account.IsActive = !account.IsActive;
            await _unitOfWork.Accounts.Update(account);
            await _unitOfWork.SaveChangesAsync();

            string status = account.IsActive ? "Activated" : "Deactivated";
            _logger.LogInformation("Account {AccountNumber} status changed to {Status} by User {UserId}.", accountNumber, status, userId);

            return new ApiResponseDto<string>
            {
                Message = $"Account status updated to {status} successfully.",
                Data = null
            };
        }
    }
}