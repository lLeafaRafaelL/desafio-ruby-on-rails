namespace ByCoders.CNAB.Core;

public record RequestDto();
public record ResponseDto();

public interface IRequestHandler<TRequest, TResponse>
    where TRequest : RequestDto
    where TResponse : ResponseDto

{
    Task<TResponse> Handle(TRequest request, CancellationToken cancellationToken);
}
