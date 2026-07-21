using Microsoft.AspNetCore.Identity;

namespace BankingSystem.Data.models
{
    public class Role : IdentityRole
    {
        public required string Description { get; set; }
        public DateTime CreatedAt { get; set; }= DateTime.UtcNow;
        public bool IsActive { get; set; } = true;
    }
}
