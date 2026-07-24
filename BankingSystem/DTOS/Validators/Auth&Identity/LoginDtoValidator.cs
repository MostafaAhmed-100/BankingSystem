using BankingSystem.DTOS.Auth_IdentityDTOs.Request_DTOs;
using FluentValidation;
using Microsoft.Extensions.Localization;

namespace BankingSystem.DTOS.Validators.Auth_Identity
{
    public class LoginDtoValidator : AbstractValidator<LoginDto>
    {
        public LoginDtoValidator()
        {
            RuleFor(x => x.Email).NotEmpty().WithMessage("Email is required.").EmailAddress();
            RuleFor(x => x.Password).NotEmpty().WithMessage("Password is required.");
        }
    }
}
