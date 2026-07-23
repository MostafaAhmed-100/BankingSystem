using BankingSystem.Data.models;
using BankingSystem.Repository.GenericRepository;

namespace BankingSystem.Repository.BankerRepository
{
    public interface IBankerRepository : IGenericRepository<Banker>
    {
        Task<Banker?> GetBankerByUserIdAsync(string userId, bool trackChanges);
        Task<(IEnumerable<Customer> Items, int TotalCount)> GetBankerPortfolioPaginatedAsync(int bankerId, int pageNumber, int pageSize);
    }
}