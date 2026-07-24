namespace BankingSystem.DTOS.CreditCards_Loans.RequestDTOs
{
    public class IssueCreditCardDto
    {
        public required int AccountNumber { get; set; }
        public required string CardType { get; set; }
        public required decimal Limit { get; set; }
    }
}
