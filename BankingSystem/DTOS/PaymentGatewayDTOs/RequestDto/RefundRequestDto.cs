namespace BankingSystem.DTOS.PaymentGatewayDTOs.RequestDto
{
    public class RefundRequestDto
    {
        public required int OriginalTransactionId { get; set; }
        public required decimal Amount { get; set; }
        public string? Reason { get; set; }
    }
}