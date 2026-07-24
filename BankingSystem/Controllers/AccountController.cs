using BankingSystem.Constants;
using BankingSystem.DTOS.Account_DTOs.Request_DTOs;
using BankingSystem.DTOS.Shared;
using BankingSystem.Services.AccountService;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace BankingSystem.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(AppRoles.Customer)] 
    public class AccountController : ControllerBase
    {
        private readonly IAccountService _accountService;

        public AccountController(IAccountService accountService)
        {
            _accountService = accountService;
        }

        [HttpPost("create")]
        public async Task<IActionResult> CreateAccount([FromBody] CreateAccountDto requestDto)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var response = await _accountService.CreateAccountAsync(requestDto, userId);
            return Ok(response);
        }

        [HttpGet("{accountNumber}/balance")]
        public async Task<IActionResult> GetBalance(int accountNumber)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var response = await _accountService.GetAccountBalanceAsync(accountNumber, userId);
            return Ok(response);
        }

        [HttpGet("my-accounts")]
        public async Task<IActionResult> GetMyAccounts([FromQuery] PaginationRequestDto paginationDto)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var response = await _accountService.GetCustomerAccountsAsync(userId, paginationDto);
            return Ok(response);
        }

        [HttpPatch("{accountNumber}/toggle-status")]
        public async Task<IActionResult> ToggleStatus(int accountNumber)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var response = await _accountService.ToggleAccountStatusAsync(accountNumber, userId);
            return Ok(response);
        }
    }
}