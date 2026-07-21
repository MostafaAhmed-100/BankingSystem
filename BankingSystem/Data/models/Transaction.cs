namespace BankingSystem.Data.models
{
    public class Transaction
    {
        public int Id { get; set; }
        public required decimal Amount { get; set; }
        public required string Type { get; set; }
        public DateTime TransactionDate { get; set; } = DateTime.UtcNow;

        public int AccountNumber { get; set; }
        public Account Account { get; set; }
    }
}
