using BankingSystem.DTOS.CreditCards_Loans.RequestDTOs;
using FluentValidation;

namespace BankingSystem.DTOS.Validators.CreditCards_Loans
{
    public class ApplyForLoanDtoValidator : AbstractValidator<ApplyForLoanDto>
    {
        public ApplyForLoanDtoValidator()
        {
            RuleFor(x => x.Amount).GreaterThan(1000).WithMessage("Loan amount must be greater than 1000.");
            RuleFor(x => x.BranchId).NotEmpty().WithMessage("Branch ID is required.");
        }
    }
}
