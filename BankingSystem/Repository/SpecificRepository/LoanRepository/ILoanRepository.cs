using BankingSystem.Data.models;
using BankingSystem.Repository.GenericRepository;

namespace BankingSystem.Repository.LoanRepository
{
    public interface ILoanRepository : IGenericRepository<Loan>
    {
        Task<(IEnumerable<Loan> Items, int TotalCount)> GetLoansByCustomerIdPaginatedAsync(int customerId, int pageNumber, int pageSize, bool trackChanges);
    }
}