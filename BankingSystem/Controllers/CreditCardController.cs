using BankingSystem.Constants;
using BankingSystem.DTOS.CreditCards_Loans.RequestDTOs;
using BankingSystem.DTOS.Shared;
using BankingSystem.DTOS.Shared.RequestDto;
using BankingSystem.Service.CreditCardService;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace BankingSystem.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = AppRoles.Customer)]
    public class CreditCardController : ControllerBase
    {
        private readonly ICreditCardService _creditCardService;

        public CreditCardController(ICreditCardService creditCardService)
        {
            _creditCardService = creditCardService;
        }

        [HttpPost("issue")]
        public async Task<IActionResult> IssueCard([FromBody] IssueCreditCardDto requestDto)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var response = await _creditCardService.IssueNewCardAsync(requestDto, userId);
            return Ok(response);
        }

        [HttpPatch("{cardNumber}/toggle-status")]
        public async Task<IActionResult> ToggleStatus(string cardNumber)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var response = await _creditCardService.ToggleCardStatusAsync(cardNumber, userId);
            return Ok(response);
        }

        [HttpGet("my-cards")]
        public async Task<IActionResult> GetMyCards([FromQuery] PaginationRequestDto paginationDto)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var response = await _creditCardService.GetMyCardsAsync(userId, paginationDto);
            return Ok(response);
        }
    }
}