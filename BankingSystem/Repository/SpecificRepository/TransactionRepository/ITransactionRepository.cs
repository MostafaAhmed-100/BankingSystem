using BankingSystem.Data.models;
using BankingSystem.Repository.GenericRepository;

namespace BankingSystem.Repository.SpecificRepository.TransactionRepository
{
    public interface ITransactionRepository : IGenericRepository<Transaction>
    {
        Task<(IEnumerable<Transaction> Items, int TotalCount)> GetTransactionsByAccountIdPaginatedAsync(int accountNumber, int pageNumber, int pageSize, bool trackChanges);
        Task<bool> ExistsByReferenceIdAsync(int referenceId);
        Task<(IEnumerable<Transaction> Items, int TotalCount)> GetTransactionsByDateRangePaginatedAsync(int accountNumber, DateTime startDate, DateTime endDate, int pageNumber, int pageSize, bool trackChanges);
    }
}