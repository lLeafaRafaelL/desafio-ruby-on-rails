using ByCoders.CNAB.Core;
using ByCoders.CNAB.Core.Validators;
using ByCoders.CNAB.Domain.Transactions;
using Microsoft.Extensions.Caching.Memory;

namespace ByCoders.CNAB.Application.Transactions.FindTransactions;

public class TransactionStatementHandler : RequestHandler<TransactionStatementRequest, TransactionStatementResponse>
{
    private readonly ITransactionRepository _repository;
    private readonly IDtoValidator<TransactionStatementRequest> _validator;
    private readonly IMemoryCache _memoryCache;

    public TransactionStatementHandler(
        ITransactionRepository repository,
        IDtoValidator<TransactionStatementRequest> validator,
        IMemoryCache memoryCache)
    {
        _repository = repository ?? throw new ArgumentNullException(nameof(repository));
        _validator = validator ?? throw new ArgumentNullException(nameof(validator));
        _memoryCache = memoryCache ?? throw new ArgumentNullException(nameof(memoryCache));
    }

    public async override Task<RequestHandlerResult<TransactionStatementResponse>> HandleAsync(TransactionStatementRequest request, CancellationToken cancellationToken)
    {
        var validationResult = _validator.TryValidate(request);
        if (validationResult.IsValid is false)
            return RequestHandlerResult<TransactionStatementResponse>.Unprocessable(failureDetails: validationResult.FailureDetails);

        if (_memoryCache.TryGetValue(request.IdempotencyKey, out TransactionStatementResponse response) is false)
        {
            var transactions = await _repository.FindBy(request.StoreName, request.StartDate, request.EndDate, cancellationToken);

            if (transactions is null)
                return RequestHandlerResult<TransactionStatementResponse>.NoContent();

            response = new TransactionStatementResponse(request.StartDate, request.EndDate, transactions);

            _memoryCache.Set(request.IdempotencyKey, response, TimeSpan.FromMinutes(1));
        }

        return RequestHandlerResult<TransactionStatementResponse>.Success(response);
    }
}