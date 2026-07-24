using BankingSystem.DTOS.Account_DTOs.Request_DTOs;
using FluentValidation;
using Microsoft.Extensions.Localization;

namespace BankingSystem.DTOS.Validators.Account
{
    public class CreateAccountDtoValidator : AbstractValidator<CreateAccountDto>
    {
        public CreateAccountDtoValidator()
        {
            RuleFor(x => x.CurrencyCode)
                .NotEmpty().WithMessage("Currency Code Is Requerd")
                .Length(3).WithMessage("Currency Code Length Is Invalid");

            RuleFor(x => x.InitialDeposit)
                .GreaterThanOrEqualTo(0).WithMessage("Invalid Initial Deposit");
        }
    }
}
