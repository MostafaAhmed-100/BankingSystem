namespace BankingSystem.DTOS.Shared
{
    public class PaginatedResponseDto<T>
    {
        public int PageSize { get; set; }
        public int CurrentPage { get; set; }
        public int TotalPages { get; set; }
        public int TotalCount { get; set; }
        public List<T?> Data { get; set; }
    }
}
