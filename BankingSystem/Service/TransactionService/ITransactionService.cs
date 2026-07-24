using BankingSystem.DTOS.Shared;
using BankingSystem.DTOS.Transactions_TransfersDomain;
using BankingSystem.DTOS.Transactions_TransfersDomain.Request_DTOs;
using BankingSystem.DTOS.Transactions_TransfersDomain.Response_DTOs;

namespace BankingSystem.Services.TransactionService
{
    public interface ITransactionService
    {
        Task<ApiResponseDto<TransactionResponseDto>> TransferFundsAsync(TransferFundsDto requestDto, string userId);
        Task<ApiResponseDto<TransactionResponseDto>> DepositAsync(DepositWithdrawDto requestDto, string userId);
        Task<ApiResponseDto<PaginatedResponseDto<TransactionResponseDto>>> GetAccountStatementAsync(int accountNumber, PaginationRequestDto paginationDto, string userId);
    }
}