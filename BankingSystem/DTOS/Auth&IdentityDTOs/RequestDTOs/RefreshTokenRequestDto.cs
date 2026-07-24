using System.ComponentModel.DataAnnotations;

namespace BankingSystem.DTOS.Auth_IdentityDTOs.RequestDTOs
{
    public class RefreshTokenRequestDto
    {
        [Required]
        public required string Token { get; set; }
    }
}
