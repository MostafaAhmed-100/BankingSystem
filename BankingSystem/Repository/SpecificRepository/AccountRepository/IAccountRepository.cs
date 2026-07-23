using BankingSystem.Data.models;
using BankingSystem.Repository.GenericRepository;

namespace BankingSystem.Repository.SpecificRepository.AccountRepository
{
    public interface IAccountRepository : IGenericRepository<Account>
    {
        Task<Account?> GetAccountByNumberAsync(int accountNumber, bool trackChanges);
        Task<IEnumerable<Account>> GetAccountsByCustomerIdAsync(int customerId, bool trackChanges);
    }
}