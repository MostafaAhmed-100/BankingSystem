namespace BankingSystem.Data.models
{
    public class Loan
    {
        public int LoanNumber { get; set; }
        public required Decimal Amount { get; set; }
        public required int InterestRate { get; set; }
        public int CustomerId { get; set; }
        public required int BranchId { get; set; }
        public Branch Branch { get; set; }
        public Customer Customer { get; set; }

    }
}
