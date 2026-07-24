namespace BankingSystem.DTOS.PaymentGateway.ResponseDto
{
    public class PaymentResultResponseDto
    {
        public bool IsSuccessful { get; set; }
        public int? TransactionId { get; set; }
        public string Message { get; set; }
    }
}
