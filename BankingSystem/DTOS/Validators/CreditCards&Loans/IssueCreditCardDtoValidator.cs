using BankingSystem.DTOS.CreditCards_Loans.RequestDTOs;
using FluentValidation;

namespace BankingSystem.DTOS.Validators.CreditCards_Loans
{
    public class IssueCreditCardDtoValidator : AbstractValidator<IssueCreditCardDto>
    {
        public IssueCreditCardDtoValidator()
        {
            RuleFor(x => x.AccountNumber).NotEmpty().WithMessage("Account number is required.");
            RuleFor(x => x.CardType).NotEmpty().WithMessage("Card type is required.").MaximumLength(50);
            RuleFor(x => x.Limit).GreaterThanOrEqualTo(0).WithMessage("Limit cannot be negative.");
        }
    }
}
