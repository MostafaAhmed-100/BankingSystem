using BankingSystem.DTOS.Transactions_TransfersDomain.Request_DTOs;
using FluentValidation;

namespace BankingSystem.DTOS.Validators.Transactions_TransfersDomain
{
    public class DepositWithdrawDtoValidator : AbstractValidator<DepositWithdrawDto>
    {
        public DepositWithdrawDtoValidator()
        {
            RuleFor(x => x.AccountNumber).NotEmpty().WithMessage("Account number is required.");
            RuleFor(x => x.Amount).GreaterThan(0).WithMessage("Amount must be greater than zero.");
        }
    }
}
