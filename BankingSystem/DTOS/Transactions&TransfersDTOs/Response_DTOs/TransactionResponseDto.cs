namespace BankingSystem.DTOS.Transactions_TransfersDomain.Response_DTOs
{
    public class TransactionResponseDto
    {
        public int Id { get; set; }
        public decimal Amount { get; set; }
        public string Type { get; set; }
        public DateTime TransactionDate { get; set; }
    }
}
