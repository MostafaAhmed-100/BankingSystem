namespace BankingSystem.DTOS.Auth_IdentityDTOs.ResponseDto
{
    public class AuthResponseDto
    {
        public required string Token { get; set; }
        public required DateTime Expiration { get; set; }
        public required string Email { get; set; }
        public required string Role { get; set; }
        public required string RefreshToken { get; set; }

        public required DateTime RefreshTokenExpiration { get; set; }
    }
}
