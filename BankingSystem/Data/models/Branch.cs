namespace BankingSystem.Data.models
{
    public class Branch
    {
        public int Id { get; set; }
        public required string Code { get; set; }
        public ICollection<Loan> Loans { get; set; } = new List<Loan>();
    }
}
