using BankingSystem.Data.models;
using BankingSystem.Repository.GenericRepository;

namespace BankingSystem.Repository.SpecificRepository.AccountRepository
{
    public interface IAccountRepository : IGenericRepository<Account>
    {
        Task<Account?> GetAccountByNumberAsync(int accountNumber, bool trackChanges);
        Task<IEnumerable<Account>> GetAccountsByCustomerIdAsync(int customerId, bool trackChanges);
        Task<(IEnumerable<Account> accounts, int totalCount)> GetAccountsByCustomerIdPaginatedAsync(int customerId, int pageNumber, int pageSize, bool trackChanges);
    }
}