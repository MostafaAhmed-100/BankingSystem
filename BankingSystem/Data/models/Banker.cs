namespace BankingSystem.Data.models
{
    public class Banker
    {
        public int Id { get; set; }
        public required string Name { get; set; }
        public bool IsActive { get; set; } = true;
        public required string UserId { get; set; }
        public required User user { get; set; }
        public int BranchId { get; set; }
        public Branch Branch { get; set; }

        public ICollection<Customer> customers { get; set; } = new List<Customer>();
    }
}
