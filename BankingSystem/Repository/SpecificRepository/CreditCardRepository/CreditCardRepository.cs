using BankingSystem.Data;
using BankingSystem.Data.models;
using BankingSystem.Repository.GenericRepository;
using Microsoft.EntityFrameworkCore;

namespace BankingSystem.Repository.CreditCardRepository
{
    public class CreditCardRepository : GenericRepository<CreditCard>, ICreditCardRepository
    {
        public CreditCardRepository(AppDbContext appDbcontext) : base(appDbcontext)
        {
        }

        public async Task<CreditCard?> GetCardByNumberAsync(string cardNumber, bool trackChanges)
        {
            var query = _AppDbcontext.CreditCards.AsQueryable();

            if (!trackChanges)
                query = query.AsNoTracking();

            return await query.FirstOrDefaultAsync(c => c.CardNumber == cardNumber);
        }

        public async Task<(IEnumerable<CreditCard> Items, int TotalCount)> GetCardsByAccountIdPaginatedAsync(int accountNumber, int pageNumber, int pageSize, bool trackChanges)
        {
            var query = _AppDbcontext.CreditCards.Where(c => c.AccountNumber == accountNumber);

            if (!trackChanges)
                query = query.AsNoTracking();

            return await ApplyPaginationAsync(query, pageNumber, pageSize);
        }
        public async Task<(IEnumerable<CreditCard> cards, int totalCount)> GetCardsByCustomerIdPaginatedAsync(int customerId, int pageNumber, int pageSize, bool trackChanges)
        {
            var query = _AppDbcontext.CreditCards.Where(c => c.CustomerId == customerId);

            if (!trackChanges)
            {
                query = query.AsNoTracking();
            }
            int totalCount = await query.CountAsync();

            var cards = await query
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return (cards, totalCount);
        }
    }
}