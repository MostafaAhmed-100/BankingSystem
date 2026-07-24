namespace BankingSystem.DTOS.Transactions_TransfersDomain
{
    public class TransferFundsDto
    {
        public required int FromAccountNumber { get; set; }
        public required int ToAccountNumber { get; set; }
        public required decimal Amount { get; set; }
    }
}
