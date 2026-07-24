using BankingSystem.DTOS.Auth_IdentityDTOs.Request_DTOs;
using FluentValidation;
using Microsoft.Extensions.Localization;

namespace BankingSystem.DTOS.Validators.Auth_Identity
{
    public class RegisterBankerDtoValidator : AbstractValidator<RegisterBankerDto>
    {
        public RegisterBankerDtoValidator()
        {
            RuleFor(x => x.Name).NotEmpty().WithMessage("Name is required.").MaximumLength(100);
            RuleFor(x => x.Email).NotEmpty().WithMessage("Email is required.").EmailAddress().WithMessage("Invalid email format.");
            RuleFor(x => x.Password).NotEmpty().WithMessage("Password is required.").MinimumLength(7);
            RuleFor(x => x.BranchId).NotEmpty().WithMessage("Branch ID is required.");
        }
    }
}
