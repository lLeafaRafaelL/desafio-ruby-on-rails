using ByCoders.CNAB.Core.Results;

namespace ByCoders.CNAB.Core;

public readonly struct RequestHandlerResult<TResultValue>
    where TResultValue : Dto
{
    private Result<TResultValue> Result { get; }

    public readonly bool Succeeded => Result.Succeeded;

    public readonly TResultValue? Value => Result.Value;

    public readonly IReadOnlyCollection<ResultFailureDetail> FailureDetails => Result.FailureDetails;

    public readonly RequestHandlerStatus Status { get; }

    private RequestHandlerResult(Result<TResultValue> result, RequestHandlerStatus status)
    {
        Status = status;
        Result = result;
    }

    public Result AsResult() => Result;

    public static RequestHandlerResult<TResultValue> NoContent() =>
        new(Result<TResultValue>.Success(), RequestHandlerStatus.NoContent);

    public static RequestHandlerResult<TResultValue> Created(TResultValue? value = default) =>
        new(Result<TResultValue>.Success(value), RequestHandlerStatus.Created);

    public static RequestHandlerResult<TResultValue> Accepted(TResultValue? value = default) =>
        new(Result<TResultValue>.Success(value), RequestHandlerStatus.Accepted);
    public static RequestHandlerResult<TResultValue> Success(TResultValue? value = default) =>
        new(Result<TResultValue>.Success(value), RequestHandlerStatus.OK);

    public static RequestHandlerResult<TResultValue> Unprocessable(TResultValue? value = default, IEnumerable<ResultFailureDetail>? failureDetails = default) =>
        new(Result<TResultValue>.Failure(value, failureDetails), RequestHandlerStatus.Unprocessable);

    public static RequestHandlerResult<TResultValue> Unprocessable(params string[] descriptions) =>
        new(Result<TResultValue>.Failure(descriptions), RequestHandlerStatus.Unprocessable);

    public static RequestHandlerResult<TResultValue> Unprocessable(IEnumerable<ResultFailureDetail> dtoValidationDetails) =>
        new(Result<TResultValue>.Failure(failureDetails: dtoValidationDetails.Select(detail => new ResultFailureDetail(detail.Description, detail.Tag))), RequestHandlerStatus.Unprocessable);

    public static RequestHandlerResult<TResultValue> NotFound(TResultValue? value = default, IEnumerable<ResultFailureDetail>? failureDetails = default) =>
        new(Result<TResultValue>.Failure(value, failureDetails), RequestHandlerStatus.NotFound);

    public static RequestHandlerResult<TResultValue> NotFound(params string[] descriptions) =>
        new(Result<TResultValue>.Failure(descriptions), RequestHandlerStatus.NotFound);
}