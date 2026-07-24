using AutoMapper;
using BankingSystem.Data.models;
using BankingSystem.DTOS.CreditCards_Loans.RequestDTOs;
using BankingSystem.DTOS.CreditCards_Loans.ResponseDto;
using BankingSystem.DTOS.CreditCards_LoansDTOs.RequestDTOs;
using BankingSystem.DTOS.Shared;
using BankingSystem.Exceptions;
using BankingSystem.Repository.GenericRepository.UnitOfWork;
using BankingSystem.Service.CreditCardService;
using Microsoft.Extensions.Logging;

namespace BankingSystem.Services.CreditCardService
{
    public class CreditCardService : ICreditCardService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<CreditCardService> _logger;
        private readonly IMapper _mapper;

        public CreditCardService(
            ILogger<CreditCardService> logger,
            IMapper mapper,
            IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
            _mapper = mapper;
        }

        public async Task<ApiResponseDto<CreditCardResponseDto>> IssueNewCardAsync(IssueCreditCardDto requestDto, string userId)
        {
            var customer = await _unitOfWork.Customers.GetCustomerByUserIdAsync(userId, false);
            if (customer == null || !customer.IsActive)
            {
                throw new UnauthorizedException("User profile not found or inactive.");
            }

            var account = await _unitOfWork.Accounts.GetAccountByNumberAsync(requestDto.AccountNumber, trackChanges: false);

            if (account == null || !account.IsActive)
            {
                throw new NotFoundException("Account not found or inactive.");
            }

            if (account.CustomerId != customer.Id)
            {
                _logger.LogWarning("Security Warning: User {UserId} attempted to issue a card for unauthorized Account {AccountNumber}.", userId, requestDto.AccountNumber);
                throw new UnauthorizedException("You are not authorized to issue a card for this account.");
            }

            string newCardNumber = GenerateRandomCardNumber();
            string newCvv = new Random().Next(100, 999).ToString();

            var creditCard = new CreditCard
            {
                CardNumber = newCardNumber,
                CVV = newCvv,
                CardType = requestDto.CardType,
                Limit = requestDto.Limit,
                ExpireDate = DateTime.UtcNow.AddYears(3),
                IsActive = true,
                CreatedAt = DateTime.UtcNow,
                AccountNumber = account.AccountNumber,
                CustomerId = customer.Id
            };

            await _unitOfWork.CreditCards.AddAsync(creditCard);
            await _unitOfWork.SaveChangesAsync();

            _logger.LogInformation("Credit Card {CardNumber} issued successfully for Account {AccountNumber} (User {UserId}).",
                MaskCardNumber(newCardNumber), account.AccountNumber, userId);

            return new ApiResponseDto<CreditCardResponseDto>
            {
                Message = "Credit card issued successfully.",
                Data = _mapper.Map<CreditCardResponseDto>(creditCard)
            };
        }

        public async Task<ApiResponseDto<string>> ToggleCardStatusAsync(string cardNumber, string userId)
        {
            var customer = await _unitOfWork.Customers.GetCustomerByUserIdAsync(userId, false);
            if (customer == null)
            {
                throw new UnauthorizedException("User profile not found.");
            }

            var creditCard = await _unitOfWork.CreditCards.GetCardByNumberAsync(cardNumber, trackChanges: true);

            if (creditCard == null)
            {
                throw new NotFoundException("Credit card not found.");
            }

            if (creditCard.CustomerId != customer.Id)
            {
                _logger.LogWarning("Security Warning: User {UserId} attempted to toggle status for unauthorized Card.", userId);
                throw new UnauthorizedException("You are not authorized to modify this credit card.");
            }

            creditCard.IsActive = !creditCard.IsActive;
            await _unitOfWork.CreditCards.Update(creditCard);
            await _unitOfWork.SaveChangesAsync();

            string status = creditCard.IsActive ? "Activated" : "Deactivated";
            _logger.LogInformation("Credit Card status changed to {Status} by User {UserId}.", status, userId);

            return new ApiResponseDto<string>
            {
                Message = $"Credit card status updated to {status} successfully.",
                Data = null
            };
        }

        public async Task<ApiResponseDto<PaginatedResponseDto<CreditCardResponseDto>>> GetMyCardsAsync(string userId, PaginationRequestDto paginationDto)
        {
            var customer = await _unitOfWork.Customers.GetCustomerByUserIdAsync(userId, false);
            if (customer == null)
            {
                throw new NotFoundException("Customer profile not found.");
            }

            var (cards, totalCount) = await _unitOfWork.CreditCards.GetCardsByCustomerIdPaginatedAsync(
                customer.Id,
                paginationDto.PageNumber,
                paginationDto.PageSize,
                trackChanges: false
            );

            int totalPages = (int)Math.Ceiling(totalCount / (double)paginationDto.PageSize);

            var mappedCards = _mapper.Map<List<CreditCardResponseDto>>(cards);

            return new ApiResponseDto<PaginatedResponseDto<CreditCardResponseDto>>
            {
                Message = "Cards retrieved successfully.",
                Data = new PaginatedResponseDto<CreditCardResponseDto>
                {                
                    CurrentPage = paginationDto.PageNumber,
                    PageSize = paginationDto.PageSize,
                    TotalCount = totalCount,
                    TotalPages = totalPages,
                    Data = mappedCards
                
                }
            };
        }
        public async Task<ApiResponseDto<bool>> ValidateCardAsync(ValidateCardRequestDto requestDto)
        {
            var creditCard = await _unitOfWork.CreditCards.GetCardByNumberAsync(requestDto.CardNumber, trackChanges: false);

            if (creditCard == null || !creditCard.IsActive)
            {
                _logger.LogWarning("Card validation failed: Card {CardNumber} not found or inactive.", MaskCardNumber(requestDto.CardNumber));
                return new ApiResponseDto<bool>
                {
                    IsSuccess = false,
                    StatusCode = 404,
                    Message = "Card is invalid or inactive.",
                    Data = false
                };
            }

            if (creditCard.CVV != requestDto.CVV ||
                creditCard.ExpireDate.Month != requestDto.ExpireDate.Month ||
                creditCard.ExpireDate.Year != requestDto.ExpireDate.Year)
            {
                _logger.LogWarning("Card validation failed: Authentication details mismatch for Card {CardNumber}.", MaskCardNumber(requestDto.CardNumber));
                return new ApiResponseDto<bool>
                {
                    IsSuccess = false,
                    StatusCode = 400,
                    Message = "Card validation details (CVV or Expiry) do not match.",
                    Data = false
                };
            }

            if (creditCard.ExpireDate < DateTime.UtcNow)
            {
                return new ApiResponseDto<bool>
                {
                    IsSuccess = false,
                    StatusCode = 400,
                    Message = "Card has expired.",
                    Data = false
                };
            }

            return new ApiResponseDto<bool>
            {
                IsSuccess = true,
                StatusCode = 200,
                Message = "Card is valid and active.",
                Data = true
            };
        }

        #region Private Helper Methods

        private string GenerateRandomCardNumber()
        {
            var random = new Random();
            string start = random.Next(0, 2) == 0 ? "4" : "5";
            string rest = string.Empty;

            for (int i = 0; i < 15; i++)
            {
                rest += random.Next(0, 10).ToString();
            }

            return start + rest;
        }

        private string MaskCardNumber(string cardNumber)
        {
            if (string.IsNullOrEmpty(cardNumber) || cardNumber.Length < 4)
                return cardNumber;
            return new string('*', cardNumber.Length - 4) + cardNumber.Substring(cardNumber.Length - 4);
        }

        #endregion
    }
}