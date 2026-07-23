using BankingSystem.Data;
using BankingSystem.Data.models;
using BankingSystem.Repository.GenericRepository;
using Microsoft.EntityFrameworkCore;

namespace BankingSystem.Repository.LoanRepository
{
    public class LoanRepository : GenericRepository<Loan>, ILoanRepository
    {
        public LoanRepository(AppDbContext appDbcontext) : base(appDbcontext)
        {
        }

        public async Task<(IEnumerable<Loan> Items, int TotalCount)> GetLoansByCustomerIdPaginatedAsync(int customerId, int pageNumber, int pageSize, bool trackChanges)
        {
            var query = _AppDbcontext.Loans.Where(l => l.CustomerId == customerId);

            if (!trackChanges)
                query = query.AsNoTracking();

            return await ApplyPaginationAsync(query, pageNumber, pageSize);
        }
    }
}