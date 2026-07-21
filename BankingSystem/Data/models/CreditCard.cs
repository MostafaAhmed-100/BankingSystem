namespace BankingSystem.Data.models
{
    public class CreditCard
    {
        public string CardNumber { get; set; }
        public string CardType { get; set; }
        public required DateTime ExpireDate { get; set; }
        public bool IsActive { get; set; } = true;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public Decimal Limit { get; set; }
        public int AccountNumber { get; set; }
        public int CustomerId { get; set; }
        public Account Account { get; set; }
        public Customer Customer { get; set; }
    }
}
