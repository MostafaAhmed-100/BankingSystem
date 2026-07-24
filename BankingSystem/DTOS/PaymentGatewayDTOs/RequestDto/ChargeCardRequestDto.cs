namespace BankingSystem.DTOS.PaymentGateway.RequestDto
{
    public class ChargeCardRequestDto
    {
        public required string CardNumber { get; set; }
        public required string CVV { get; set; }
        public DateTime ExpireDate { get; set; }
        public required decimal Amount { get; set; }
        public required string StoreName { get; set; }
        public required int ReferenceId { get; set; }
    }
}
