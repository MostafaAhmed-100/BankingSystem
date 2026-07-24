namespace BankingSystem.DTOS.Auth_IdentityDTOs.ResponseDto
{
    public class AuthResponseDto
    {
        public required string Token { get; set; }
        public required string UserProfileId { get; set; }
        public required string Role { get; set; }
        public required DateTime Expiration { get; set; }
    }
}
