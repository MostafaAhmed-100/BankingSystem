using Microsoft.AspNetCore.Identity;

namespace BankingSystem.Data.models
{
    public class User : IdentityUser
    {
        public Customer? Customer { get; set; }
        public Banker? Banker { get; set; }
    }
}
