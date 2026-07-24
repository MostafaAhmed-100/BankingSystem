using BankingSystem.DTOS.Account_DTOs.ResponseDto;
using BankingSystem.DTOS.Transactions_TransfersDomain.Response_DTOs;

namespace BankingSystem.DTOS.Account_DTOs.Request_DTOs
{
    public class AccountWithTransactionsDto : AccountResponseDto
    {
        public IEnumerable<TransactionResponseDto> Transactions { get; set; }
    }
}
