using System.ComponentModel.DataAnnotations;
namespace BankingSystem.Data.models
{
    public class Customer
    {
        public string Id { get; set; }

        public required string Name { get; set; }

        public required string Street { get; set; }

        public required string City { get; set; }

        public bool IsActive = true;

        public string UserId { get; set; }
        public User user { get; set; }

        public ICollection<Loan> Loans { get; set; } = new List<Loan>();
    }
}
