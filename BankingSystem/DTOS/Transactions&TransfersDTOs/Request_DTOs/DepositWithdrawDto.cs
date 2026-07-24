namespace BankingSystem.DTOS.Transactions_TransfersDomain.Request_DTOs
{
    public class DepositWithdrawDto
    {
        public required int AccountNumber { get; set; }
        public required decimal Amount { get; set; }
    }
}
