using BankingSystem.Data.models;
using BankingSystem.Repository.GenericRepository;

namespace BankingSystem.Repository.SpecificRepository.CustomerRepository
{
    public interface ICustomerRepository : IGenericRepository<Customer>
    {
        Task<Customer?> GetCustomerByUserIdAsync(string userId, bool trackChanges);
        Task<(IEnumerable<Customer> Items, int TotalCount)> GetCustomerBankersPaginatedAsync(int customerId, int pageNumber, int pageSize);
    }
}