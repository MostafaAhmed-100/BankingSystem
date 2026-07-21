using System.ComponentModel.DataAnnotations;
namespace BankingSystem.Data.models
{
    public class Customer
    {
        public int Id { get; set; }
        public required string Name { get; set; }
        public required string Street { get; set; }
        public required string City { get; set; }
        public bool IsActive { get; set; } = true;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public required string UserId { get; set; }
        public required User user { get; set; }
        public ICollection<Account> accounts { get; set; } = new List<Account>();
        public ICollection<Loan> Loans { get; set; } = new List<Loan>();
        public ICollection<Account> Accounts { get; set; } = new List<Account>();
        public ICollection<Banker> Bankers { get; set; } = new List<Banker>();
    }
}
