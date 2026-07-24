using BankingSystem.DTOS.PaymentGateway.RequestDto;
using FluentValidation;

namespace BankingSystem.DTOS.Validators.PaymentGateway
{
    public class ChargeCardRequestDtoValidator : AbstractValidator<ChargeCardRequestDto>
    {
        public ChargeCardRequestDtoValidator()
        {
            RuleFor(x => x.CardNumber)
                .NotEmpty().WithMessage("Card Number is required.")
                .Length(16).WithMessage("Card Number must be 16 digits.")
                .Matches("^[0-9]+$").WithMessage("Card Number must contain only numbers.");
            RuleFor(x => x.ExpireDate).NotEmpty().WithMessage("Expiration Date is required.");
            RuleFor(x => x.Amount).GreaterThan(0).WithMessage("Amount must be greater than zero.");
        }
    }
}
