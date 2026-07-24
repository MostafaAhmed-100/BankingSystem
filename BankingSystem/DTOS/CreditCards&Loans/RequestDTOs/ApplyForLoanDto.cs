namespace BankingSystem.DTOS.CreditCards_Loans.RequestDTOs
{
    public class ApplyForLoanDto
    {
        public required decimal Amount { get; set; }
        public required int BranchId { get; set; }
    }
}
