namespace BankingSystem.DTOS.Account_DTOs.ResponseDto
{
    public class AccountResponseDto
    {
        public string CurrencyCode { get; set; }
        public decimal Balance { get; set; }
        public int AccountNumber { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }

    }
}
