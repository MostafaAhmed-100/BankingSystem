using BankingSystem.Data;
using BankingSystem.Repository.BankerRepository;
using BankingSystem.Repository.CreditCardRepository;
using BankingSystem.Repository.LoanRepository;
using BankingSystem.Repository.SpecificRepository.AccountRepository;
using BankingSystem.Repository.SpecificRepository.CustomerRepository;
using BankingSystem.Repository.SpecificRepository.TransactionRepository;
using Microsoft.EntityFrameworkCore.Storage;

namespace BankingSystem.Repository.UnitOfWork
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly AppDbContext _context;

        public IAccountRepository Accounts { get; private set; }
        public ITransactionRepository Transactions { get; private set; }
        public ICustomerRepository Customers { get; private set; }
        public IBankerRepository Bankers { get; private set; }
        public ICreditCardRepository CreditCards { get; private set; }
        public ILoanRepository Loans { get; private set; }

        public UnitOfWork(AppDbContext context)
        {
            _context = context;
            Accounts = new AccountRepository(_context);
            Transactions = new TransactionRepository(_context);
            Customers = new CustomerRepository(_context);
            Bankers = new BankerRepository.BankerRepository(_context);
            CreditCards = new CreditCardRepository.CreditCardRepository(_context);
            Loans = new LoanRepository.LoanRepository(_context);
        }

        public async Task<int> CompleteAsync()
        {
            return await _context.SaveChangesAsync();
        }

        public async Task<IDbContextTransaction> BeginTransactionAsync()
        {
            return await _context.Database.BeginTransactionAsync();
        }

        public void Dispose()
        {
            _context.Dispose();
        }
    }
}