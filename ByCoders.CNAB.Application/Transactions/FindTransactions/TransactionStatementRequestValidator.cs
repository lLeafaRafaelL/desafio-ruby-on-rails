using ByCoders.CNAB.Core.Validators;
using FluentValidation;

namespace ByCoders.CNAB.Application.Transactions.FindTransactions;

internal class TransactionStatementRequestValidator : FluentDtoValidator<TransactionStatementRequest>
{
    public TransactionStatementRequestValidator()
    {
        RuleFor(x => x.StoreName)
            .NotEmpty()
            .MaximumLength(19);

        RuleFor(x => x.StartDate)
            .NotEmpty()
            .LessThan(x => x.EndDate)
            .Must(x => x > DateTime.MinValue);

        RuleFor(x => x.EndDate)
            .NotEmpty()
            .GreaterThan(x => x.StartDate)
            .Must(x => x > DateTime.MinValue);

        RuleFor(x => x)
            .Must(x => x.EndDate.Subtract(x.StartDate).TotalDays <= 1)
            .WithMessage("The maximum consultation period is 1 day.");

    }
}