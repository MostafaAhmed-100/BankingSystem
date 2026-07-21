namespace BankingSystem.Data.models
{
    public class Account
    {
        public int AccountNumber { get; set; }
        public required decimal Balance { get; set; }
        public required string CurrencyCode { get; set; }
        public required byte[] RowVersion { get; set; }
        public bool IsActive { get; set; } = true;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public int CustomerId { get; set; }
        public Customer Customer { get; set; }

        public ICollection<CreditCard> CreditCards { get; set; } = new List<CreditCard>();
        public ICollection<Transaction> Transactions { get; set; } = new List<Transaction>();
    }
}
