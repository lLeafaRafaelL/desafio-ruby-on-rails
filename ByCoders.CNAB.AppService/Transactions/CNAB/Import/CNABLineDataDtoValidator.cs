using FluentValidation;

namespace ByCoders.CNAB.AppService.Transactions.CNAB.Import;

/// <summary>
/// FluentValidation validator for CNABLineDataDto
/// Centralizes all business rules for CNAB transaction data validation
/// </summary>
public class CNABLineDataDtoValidator : AbstractValidator<CNABLineDataDto>
{
    public CNABLineDataDtoValidator()
    {
        RuleFor(x => x)
            .NotNull()
            .WithMessage("Transaction data cannot be null");

        RuleFor(x => x.CPF)
            .NotEmpty()
            .WithMessage("CPF cannot be empty")
            .Length(11)
            .WithMessage("CPF must have 11 characters");

        RuleFor(x => x.CardNumber)
            .NotEmpty()
            .WithMessage("Card number cannot be empty")
            .MinimumLength(4)
            .WithMessage("Card number must have at least 4 characters");

        RuleFor(x => x.StoreName)
            .NotEmpty()
            .WithMessage("Store name cannot be empty")
            .MaximumLength(100)
            .WithMessage("Store name must not exceed 100 characters");

        RuleFor(x => x.StoreOwner)
            .NotEmpty()
            .WithMessage("Store owner cannot be empty")
            .MaximumLength(100)
            .WithMessage("Store owner must not exceed 100 characters");

        RuleFor(x => x.Amount)
            .GreaterThanOrEqualTo(0)
            .WithMessage("Amount cannot be negative");

        RuleFor(x => x.Date)
            .NotEmpty()
            .WithMessage("Transaction date cannot be empty");

        RuleFor(x => x.Time)
            .NotEmpty()
            .WithMessage("Transaction time cannot be empty");

        RuleFor(x => x.TransactionType)
            .IsInEnum()
            .WithMessage("Invalid transaction type");
    }
}
