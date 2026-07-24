using BankingSystem.DTOS.PaymentGateway.RequestDto;
using BankingSystem.DTOS.PaymentGateway.ResponseDto;
using BankingSystem.DTOS.PaymentGatewayDTOs.RequestDto;
using BankingSystem.DTOS.Shared;

namespace BankingSystem.Service.PaymentGatewayService
{
    public interface IPaymentGatewayService
    {
        Task<ApiResponseDto<PaymentResultResponseDto>> ProcessExternalPaymentAsync(ChargeCardRequestDto requestDto);
        Task<ApiResponseDto<string>> RefundExternalPaymentAsync(RefundRequestDto requestDto);
    }
}