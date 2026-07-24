using BankingSystem.DTOS.CreditCards_LoansDTOs.RequestDTOs;
using BankingSystem.DTOS.PaymentGateway.RequestDto;
using BankingSystem.DTOS.PaymentGatewayDTOs.RequestDto;
using BankingSystem.Service.CreditCardService;
using BankingSystem.Service.PaymentGatewayService;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BankingSystem.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PaymentGatewayController : ControllerBase
    {
        private readonly IPaymentGatewayService _gatewayService;
        private readonly ICreditCardService _creditCardService; 

        public PaymentGatewayController(IPaymentGatewayService gatewayService, ICreditCardService creditCardService)
        {
            _gatewayService = gatewayService;
            _creditCardService = creditCardService;
        }

        [HttpPost("validate-card")]
        public async Task<IActionResult> ValidateCard([FromBody] ValidateCardRequestDto requestDto)
        {
            var response = await _creditCardService.ValidateCardAsync(requestDto);
            return StatusCode(response.StatusCode, response);
        }

        [HttpPost("charge")]
        public async Task<IActionResult> ProcessPayment([FromBody] ChargeCardRequestDto requestDto)
        {
            var response = await _gatewayService.ProcessExternalPaymentAsync(requestDto);
            return Ok(response);
        }

        [HttpPost("refund")]
        public async Task<IActionResult> RefundPayment([FromBody] RefundRequestDto requestDto)
        {
            var response = await _gatewayService.RefundExternalPaymentAsync(requestDto);
            return Ok(response);
        }
    }
}