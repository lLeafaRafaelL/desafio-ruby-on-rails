using ByCoders.CNAB.Core.Results;

namespace ByCoders.CNAB.Core.Validators;

public sealed class DtoValidationResult
{
    public bool IsValid { get; }

    public IReadOnlyCollection<ResultFailureDetail> FailureDetails { get; }

    public DtoValidationResult(bool succeeded)
        : this(succeeded, null)
    {
    }

    public DtoValidationResult(bool succeeded, IReadOnlyCollection<ResultFailureDetail>? failureDetails)
    {
        IsValid = succeeded;
        FailureDetails = failureDetails ?? ResultFailureDetail.EmptyFailureDetails();
    }
}