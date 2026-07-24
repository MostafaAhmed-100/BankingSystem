using BankingSystem.DTOS.Auth_IdentityDTOs.Request_DTOs;
using BankingSystem.DTOS.Auth_IdentityDTOs.RequestDTOs;
using BankingSystem.DTOS.Auth_IdentityDTOs.ResponseDto;
using BankingSystem.DTOS.Shared;
using Microsoft.AspNetCore.Identity.Data;

namespace BankingSystem.Service.AuthService
{
    public interface IAuthService
    {
        Task<ApiResponseDto<AuthResponseDto>> RegisterCustomerAsync(RegisterCustomerDto registerRequest);
        Task<ApiResponseDto<AuthResponseDto>> RegisterBankerAsync(RegisterBankerDto registerRequest);
        Task<ApiResponseDto<AuthResponseDto>> LoginAsync(LoginDto loginRequest);
        Task<ApiResponseDto<AuthResponseDto>> RefreshTokenAsync(RefreshTokenRequestDto tokenRequest);
        Task<ApiResponseDto<string>> ConfirmEmailAsync(int userId, string code);
        Task<ApiResponseDto<string>> ForgotPasswordAsync(ForgotPasswordRequestDto request);
        Task<ApiResponseDto<string>> ResetPasswordAsync(ResetPasswordRequestDto request);

    }
}
