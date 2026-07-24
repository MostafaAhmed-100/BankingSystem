namespace BankingSystem.DTOS.CreditCards_LoansDTOs.RequestDTOs
{
    public class ValidateCardRequestDto
    {
        public string CardNumber { get; set; }
        public string CVV { get; set; }
        public DateTime ExpireDate { get; set; }
    }
}
