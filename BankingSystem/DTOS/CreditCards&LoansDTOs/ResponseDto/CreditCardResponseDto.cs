namespace BankingSystem.DTOS.CreditCards_Loans.ResponseDto
{
    public class CreditCardResponseDto
    {
        public string MaskedCardNumber { get; set; }
        public string CardType { get; set; }
        public DateTime ExpireDate { get; set; }
        public decimal Limit { get; set; }
        public bool IsActive { get; set; }
    }
}
