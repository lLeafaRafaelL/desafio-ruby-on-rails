using FluentValidation;

namespace ByCoders.CNAB.AppService.Transactions.CNAB.Import;

internal class ImportCNABRequestValidator : AbstractValidator<ImportCNABRequest>
{
    public ImportCNABRequestValidator()
    {
        RuleFor(x => x.CNABFile)
            .NotNull()
            .Must(x => x.Length > 0);
    }
}