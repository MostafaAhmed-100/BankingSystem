using BankingSystem.Repository.BankerRepository;
using BankingSystem.Repository.CreditCardRepository;
using BankingSystem.Repository.LoanRepository;
using BankingSystem.Repository.SpecificRepository.AccountRepository;
using BankingSystem.Repository.SpecificRepository.CustomerRepository;
using BankingSystem.Repository.SpecificRepository.TransactionRepository;
using Microsoft.EntityFrameworkCore.Storage;

namespace BankingSystem.Repository.UnitOfWork
{
    public interface IUnitOfWork : IDisposable
    {
        IAccountRepository Accounts { get; }
        ITransactionRepository Transactions { get; }
        ICustomerRepository Customers { get; }
        IBankerRepository Bankers { get; }
        ICreditCardRepository CreditCards { get; }
        ILoanRepository Loans { get; }

        Task<int> CompleteAsync();
        Task<IDbContextTransaction> BeginTransactionAsync();
    }
}