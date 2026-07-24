using BankingSystem.DTOS.Account_DTOs.Request_DTOs;
using BankingSystem.DTOS.Account_DTOs.ResponseDto;
using BankingSystem.DTOS.Shared;

namespace BankingSystem.Services.AccountService
{
    public interface IAccountService
    {
        Task<ApiResponseDto<AccountResponseDto>> CreateAccountAsync(CreateAccountDto requestDto, string userId);
        Task<ApiResponseDto<AccountResponseDto>> GetAccountBalanceAsync(int accountNumber, string userId);
        Task<ApiResponseDto<PaginatedResponseDto<AccountResponseDto>>> GetCustomerAccountsAsync(string userId, PaginationRequestDto paginationDto);
        Task<ApiResponseDto<string>> ToggleAccountStatusAsync(int accountNumber, string userId);
    }
}