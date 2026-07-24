using BankingSystem.Constants;
using BankingSystem.DTOS.Shared;
using BankingSystem.DTOS.Transactions_TransfersDomain;
using BankingSystem.DTOS.Transactions_TransfersDomain.Request_DTOs;
using BankingSystem.Services.TransactionService;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace BankingSystem.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize (Roles = AppRoles.Customer)]
    public class TransactionController : ControllerBase
    {
        private readonly ITransactionService _transactionService;

        public TransactionController(ITransactionService transactionService)
        {
            _transactionService = transactionService;
        }

        [HttpPost("transfer")]
        public async Task<IActionResult> TransferFunds([FromBody] TransferFundsDto requestDto)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var response = await _transactionService.TransferFundsAsync(requestDto, userId);
            return Ok(response);
        }

        [HttpPost("deposit")]
        public async Task<IActionResult> Deposit([FromBody] DepositWithdrawDto requestDto)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var response = await _transactionService.DepositAsync(requestDto, userId);
            return Ok(response);
        }

        [HttpGet("{accountNumber}/statement")]
        public async Task<IActionResult> GetStatement(int accountNumber, [FromQuery] PaginationRequestDto paginationDto)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var response = await _transactionService.GetAccountStatementAsync(accountNumber, paginationDto, userId);
            return Ok(response);
        }
    }
}