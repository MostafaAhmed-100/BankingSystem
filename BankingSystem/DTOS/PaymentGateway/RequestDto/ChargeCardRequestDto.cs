namespace BankingSystem.DTOS.PaymentGateway.RequestDto
{
    public class ChargeCardRequestDto
    {
        public required string CardNumber { get; set; }
        public required DateTime ExpireDate { get; set; }
        public required decimal Amount { get; set; }
    }
}
