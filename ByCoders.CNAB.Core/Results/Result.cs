namespace ByCoders.CNAB.Core.Results;

/// <summary>
/// Result Pattern - Represents the outcome of an operation with value
/// </summary>
public readonly struct Result<TResultValue>
{
    public bool Succeeded { get; }

    public TResultValue? Value { get; }

    public IReadOnlyCollection<ResultFailureDetail> FailureDetails { get; }

    private Result(bool succeeded, TResultValue? value, ResultFailureDetail[]? failureDetails)
    {
        Value = value;
        Succeeded = succeeded;
        FailureDetails = failureDetails ?? ResultFailureDetail.EmptyFailureDetails();
    }

    public Result AsResult() => this ? Create() : Create(failureDetails: FailureDetails.ToArray());

    public static Result<TResultValue> Success(TResultValue? value = default) => Create(value: value);

    public static Result<TResultValue> Failure() => Create(false);

    public static Result<TResultValue> Failure(IEnumerable<ResultFailureDetail>? failureDetails) => Create(false, failureDetails: failureDetails?.ToArray());

    public static Result<TResultValue> Failure(TResultValue? value, IEnumerable<ResultFailureDetail>? failureDetails) => Create(false, value, failureDetails?.ToArray());

    public static Result<TResultValue> Failure(params string[] descriptions) => Failure(descriptions?.Select(description => new ResultFailureDetail(description)));

    internal static Result<TResultValue> Create(bool succeeded = true, TResultValue? value = default, params ResultFailureDetail[]? failureDetails)
        => new(succeeded, value, failureDetails?.ToArray());

    public static implicit operator bool(Result<TResultValue> Result) => Result.Succeeded;

    public static implicit operator Result(Result<TResultValue> Result) => Result.AsResult();
}

/// <summary>
/// Result Pattern - Represents the outcome of an operation without value
/// </summary>
public readonly struct Result
{
    public bool Succeeded { get; }

    public IReadOnlyCollection<ResultFailureDetail> FailureDetails { get; }

    private Result(bool succeeded, ResultFailureDetail[]? failureDetails)
    {
        Succeeded = succeeded;
        FailureDetails = failureDetails ?? ResultFailureDetail.EmptyFailureDetails();
    }

    public static Result Success() => new(true, default);

    public static Result Failure(IEnumerable<ResultFailureDetail>? failureDetails) =>
        new(false, failureDetails?.ToArray());

    public static Result Failure(params ResultFailureDetail[] failureDetails) =>
        new(false, failureDetails);

    public static Result Failure() => new(false, default);

    public static Result Failure(params string[] descriptions) =>
        Failure(descriptions?.Select(description => new ResultFailureDetail(description)));

    public static implicit operator bool(Result Result) => Result.Succeeded;

}