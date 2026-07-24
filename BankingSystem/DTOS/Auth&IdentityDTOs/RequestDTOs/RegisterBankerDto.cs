namespace BankingSystem.DTOS.Auth_IdentityDTOs.Request_DTOs
{
    public class RegisterBankerDto
    {
        public required string Name { get; set; }
        public required string Email { get; set; }
        public required string Password { get; set; }
        public required int BranchId { get; set; }
        public required string SecretCode { get; set; }
    }
}
