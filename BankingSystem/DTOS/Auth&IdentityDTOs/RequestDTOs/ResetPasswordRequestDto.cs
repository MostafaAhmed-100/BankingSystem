namespace BankingSystem.DTOS.Auth_IdentityDTOs.RequestDTOs
{
    public class ResetPasswordRequestDto
    {
        public string Token { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string ConfirmPassword { get; set; }
    }
}
