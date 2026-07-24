using BankingSystem.Data;
using BankingSystem.Data.models;
using BankingSystem.Repository.GenericRepository;
using Microsoft.EntityFrameworkCore;

namespace BankingSystem.Repository.SpecificRepository.TransactionRepository
{
    public class TransactionRepository : GenericRepository<Transaction>, ITransactionRepository
    {
        public TransactionRepository(AppDbContext appDbcontext) : base(appDbcontext)
        {
        }

        public async Task<bool> ExistsByReferenceIdAsync(int referenceId)
        {
            return await _AppDbcontext.Transactions.AnyAsync(t => t.ReferenceId == referenceId);
        }

        public async Task<(IEnumerable<Transaction> Items, int TotalCount)> GetTransactionsByAccountIdPaginatedAsync(int accountNumber, int pageNumber, int pageSize, bool trackChanges)
        {
            var query = _AppDbcontext.Transactions.Where(x => x.AccountNumber == accountNumber);

            if (!trackChanges)
                query = query.AsNoTracking();

            return await ApplyPaginationAsync(query, pageNumber, pageSize);
        }

        public async Task<(IEnumerable<Transaction> Items, int TotalCount)> GetTransactionsByDateRangePaginatedAsync(int accountNumber, DateTime startDate, DateTime endDate, int pageNumber, int pageSize, bool trackChanges)
        {
            var query = _AppDbcontext.Transactions
                .Where(x => x.AccountNumber == accountNumber && x.TransactionDate >= startDate && x.TransactionDate <= endDate);

            if (!trackChanges)
                query = query.AsNoTracking();

            return await ApplyPaginationAsync(query, pageNumber, pageSize);
        }
    }
}