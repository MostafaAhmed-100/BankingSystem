namespace BankingSystem.DTOS.Auth_IdentityDTOs.Request_DTOs
{
    public class LoginDto
    {
        public required string Email { get; set; }
        public required string Password { get; set; }
    }
}
