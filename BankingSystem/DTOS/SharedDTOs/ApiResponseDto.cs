namespace BankingSystem.DTOS.Shared
{
    public class ApiResponseDto<T>
    {
        public bool IsSuccess { get; set; } = true;
        public string Message { get; set; } = string.Empty;
        public int StatusCode { get; set; } = 200;
        public T? Data { get; set; }
    }
}
