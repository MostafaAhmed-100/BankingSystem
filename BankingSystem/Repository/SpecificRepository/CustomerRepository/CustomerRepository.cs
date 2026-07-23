using BankingSystem.Data;
using BankingSystem.Data.models;
using BankingSystem.Repository.GenericRepository;
using Microsoft.EntityFrameworkCore;

namespace BankingSystem.Repository.SpecificRepository.CustomerRepository
{
    public class CustomerRepository : GenericRepository<Customer>, ICustomerRepository
    {
        public CustomerRepository(AppDbContext appDbcontext) : base(appDbcontext)
        {
        }

        public async Task<Customer?> GetCustomerByUserIdAsync(string userId, bool trackChanges)
        {
            var query = _AppDbcontext.Customers.AsQueryable();

            if (!trackChanges)
                query = query.AsNoTracking();

            return await query.FirstOrDefaultAsync(c => c.UserId == userId);
        }

        public async Task<(IEnumerable<Customer> Items, int TotalCount)> GetCustomerBankersPaginatedAsync(int customerId, int pageNumber, int pageSize)
        {
            var query = _AppDbcontext.Customers
                .Where(c => c.Id == customerId)
                .Include(c => c.Bankers)
                .AsNoTracking();

            return await ApplyPaginationAsync(query, pageNumber, pageSize);
        }
    }
}