namespace BankingSystem.Repository.GenericRepository
{
    public interface IGenericRepository<T>
    {
        Task<(IEnumerable<T> Items, int TotalCount)> ApplyPaginationAsync(IQueryable<T> query, int pageNumber, int pageSize);
        Task<T?> GetByIdAsync(int id, bool trackChanges);
        Task<T?> AddAsync (T entity);
        Task AddRangeAsync(IEnumerable<T> entities);
        Task Update(T entity);
        Task DeleteAsync(T entity);
    }
}
