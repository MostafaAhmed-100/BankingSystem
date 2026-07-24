namespace BankingSystem.DTOS.Auth_IdentityDTOs.Request_DTOs
{
    public class RegisterCustomerDto
    {
        public required string Name { get; set; }
        public required string Email { get; set; }
        public required string Password { get; set; }
        public required string Street { get; set; }
        public required string City { get; set; }
    }
}
