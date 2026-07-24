using BankingSystem.DTOS.Transactions_TransfersDomain;
using FluentValidation;

namespace BankingSystem.DTOS.Validators.Transactions_TransfersDomain
{
    public class TransferFundsDtoValidator : AbstractValidator<TransferFundsDto>
    {
        public TransferFundsDtoValidator()
        {
            RuleFor(x => x.FromAccountNumber)
                .NotEmpty()
                .WithMessage("Sender account is required.");
            RuleFor(x => x.ToAccountNumber)
                .NotEmpty().WithMessage("Receiver account is required.")
                .NotEqual(x => x.FromAccountNumber).WithMessage("Cannot transfer to the same account.");
            RuleFor(x => x.Amount)
                .GreaterThan(0)
                .WithMessage("Transfer amount must be greater than zero.");
        }
    }
}
