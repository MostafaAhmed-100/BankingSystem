using BankingSystem.Data;
using BankingSystem.Data.models;
using BankingSystem.Repository.GenericRepository;
using Microsoft.EntityFrameworkCore;

namespace BankingSystem.Repository.BankerRepository
{
    public class BankerRepository : GenericRepository<Banker>, IBankerRepository
    {
        public BankerRepository(AppDbContext appDbcontext) : base(appDbcontext)
        {
        }

        public async Task<Banker?> GetBankerByUserIdAsync(string userId, bool trackChanges)
        {
            var query = _AppDbcontext.Bankers.AsQueryable();

            if (!trackChanges)
                query = query.AsNoTracking();

            return await query.FirstOrDefaultAsync(b => b.UserId == userId);
        }

        public async Task<(IEnumerable<Banker> Items, int TotalCount)> GetBankerPortfolioPaginatedAsync(int bankerId, int pageNumber, int pageSize)
        {
            var query = _AppDbcontext.Bankers
                .Where(b => b.Id == bankerId)
                .Include(b => b.customers)
                .AsNoTracking();

            return await ApplyPaginationAsync(query, pageNumber, pageSize);
        }
    }
}