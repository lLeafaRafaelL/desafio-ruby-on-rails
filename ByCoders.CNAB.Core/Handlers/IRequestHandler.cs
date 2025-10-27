using FluentValidation;

namespace ByCoders.CNAB.Core;

public record Dto();

public interface IRequestHandler<TRequest, TResponse>
    where TRequest : Dto
    where TResponse : Dto

{
    Task<RequestHandlerResult<TResponse>> HandleAsync(TRequest request, CancellationToken cancellationToken);
}


public abstract class RequestHandler<TRequest, TResponse> : IRequestHandler<TRequest, TResponse>
    where TRequest : Dto
    where TResponse : Dto
{
    private readonly IValidator<TRequest> validator;

    public abstract Task<RequestHandlerResult<TResponse>> HandleAsync(TRequest input, CancellationToken cancellationToken);
}
