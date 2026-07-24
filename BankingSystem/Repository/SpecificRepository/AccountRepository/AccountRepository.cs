using BankingSystem.Data;
using BankingSystem.Data.models;
using BankingSystem.Repository.GenericRepository;
using Microsoft.EntityFrameworkCore;

namespace BankingSystem.Repository.SpecificRepository.AccountRepository
{
    public class AccountRepository : GenericRepository<Account>, IAccountRepository
    {
        public AccountRepository(AppDbContext appDbcontext) : base(appDbcontext)
        {
        }

        public async Task<Account?> GetAccountByNumberAsync(int accountNumber, bool trackChanges)
        {
            var query = _AppDbcontext.Accounts.AsQueryable();

            if (!trackChanges)
                query = query.AsNoTracking();

            return await query.FirstOrDefaultAsync(c => c.AccountNumber == accountNumber);
        }

        public async Task<IEnumerable<Account>> GetAccountsByCustomerIdAsync(int customerId, bool trackChanges)
        {
            var query = _AppDbcontext.Accounts.Where(c => c.CustomerId == customerId);

            if (!trackChanges)
                query = query.AsNoTracking();

            return await query.ToListAsync();
        }
        public async Task<(IEnumerable<Account> accounts, int totalCount)> GetAccountsByCustomerIdPaginatedAsync(int customerId, int pageNumber, int pageSize, bool trackChanges)
        {
            var query = _AppDbcontext.Accounts.Where(a => a.CustomerId == customerId);

            if (!trackChanges)
            {
                query = query.AsNoTracking();
            }

            int totalCount = await query.CountAsync();
            var accounts = await query
                .OrderByDescending(a => a.CreatedAt) 
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return (accounts, totalCount);
        }
    }
}