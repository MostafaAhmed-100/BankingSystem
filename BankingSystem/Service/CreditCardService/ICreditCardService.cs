using BankingSystem.DTOS.CreditCards_Loans.RequestDTOs;
using BankingSystem.DTOS.CreditCards_Loans.ResponseDto;
using BankingSystem.DTOS.Shared;

namespace BankingSystem.Service.CreditCardService
{
    public interface ICreditCardService
    {
        Task<ApiResponseDto<CreditCardResponseDto>> IssueNewCardAsync(IssueCreditCardDto requestDto, string userId);
        Task<ApiResponseDto<string>> ToggleCardStatusAsync(string cardNumber, string userId);
        Task<ApiResponseDto<PaginatedResponseDto<CreditCardResponseDto>>> GetMyCardsAsync(string userId, PaginationRequestDto paginationDto);
    }
}
