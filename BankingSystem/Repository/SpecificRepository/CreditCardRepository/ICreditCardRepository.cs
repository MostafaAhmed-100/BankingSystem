using BankingSystem.Data.models;
using BankingSystem.Repository.GenericRepository;

namespace BankingSystem.Repository.CreditCardRepository
{
    public interface ICreditCardRepository : IGenericRepository<CreditCard>
    {
        Task<CreditCard?> GetCardByNumberAsync(string cardNumber, bool trackChanges);
        Task<(IEnumerable<CreditCard> Items, int TotalCount)> GetCardsByAccountIdPaginatedAsync(int accountNumber, int pageNumber, int pageSize, bool trackChanges);
        Task<(IEnumerable<CreditCard> cards, int totalCount)> GetCardsByCustomerIdPaginatedAsync(int customerId, int pageNumber, int pageSize, bool trackChanges);
    }
}