namespace BankingSystem.DTOS.Account_DTOs.Request_DTOs
{
    public class CreateAccountDto
    {
        public required string CurrencyCode { get; set; }
        public required int InitialDeposit { get; set; }
    }
}
