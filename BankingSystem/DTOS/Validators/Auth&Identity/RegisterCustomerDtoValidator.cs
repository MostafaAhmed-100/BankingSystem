using BankingSystem.DTOS.Auth_IdentityDTOs.Request_DTOs;
using FluentValidation;
using Microsoft.Extensions.Localization;

namespace BankingSystem.DTOS.Validators
{
    public class RegisterCustomerDtoValidator : AbstractValidator<RegisterCustomerDto>
    {
        public RegisterCustomerDtoValidator()
        {
            RuleFor(x => x.Name).NotEmpty().WithMessage("Name is required.").MaximumLength(100);
            RuleFor(x => x.Email).NotEmpty().WithMessage("Email is required.").EmailAddress().WithMessage("Invalid email format.");
            RuleFor(x => x.Password).NotEmpty().WithMessage("Password is required.").MinimumLength(7).WithMessage("Password must be at least 7 characters.");
            RuleFor(x => x.Street).NotEmpty().WithMessage("Street is required.").MaximumLength(150);
            RuleFor(x => x.City).NotEmpty().WithMessage("City is required.").MaximumLength(100);
        }
    }
}
