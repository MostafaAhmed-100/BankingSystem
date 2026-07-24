using System.ComponentModel.DataAnnotations;

namespace BankingSystem.DTOS.Auth_IdentityDTOs.RequestDTOs
{
    public class ForgotPasswordRequestDto
    {

        [EmailAddress]
        public required string Email { get; set; }
    }
}
