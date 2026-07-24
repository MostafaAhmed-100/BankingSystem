namespace BankingSystem.DTOS.Account_DTOs.ResponseDto
{
    public class AccountResponseDto
    {
        public required string CurrencyCode { get; set; }
        public required decimal Balance { get; set; }
        public required int AccountNumber { get; set; }
        public required bool IsActive { get; set; }
        public required DateTime CreatedAt { get; set; }

    }
}
