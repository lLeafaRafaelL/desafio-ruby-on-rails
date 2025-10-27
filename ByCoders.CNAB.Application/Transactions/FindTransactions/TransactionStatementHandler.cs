using ByCoders.CNAB.Core;
using ByCoders.CNAB.Core.Validators;
using ByCoders.CNAB.Domain.Transactions;

namespace ByCoders.CNAB.Application.Transactions.FindTransactions;

public class TransactionStatementHandler : RequestHandler<TransactionStatementRequest, TransactionStatementResponse>
{
    private readonly ITransactionRepository _repository;
    private readonly IDtoValidator<TransactionStatementRequest> _validator;

    public TransactionStatementHandler(
        ITransactionRepository repository,
        IDtoValidator<TransactionStatementRequest> validator)
    {
        _repository = repository ?? throw new ArgumentNullException(nameof(repository));
        _validator = validator ?? throw new ArgumentNullException(nameof(validator));
    }

    public async override Task<RequestHandlerResult<TransactionStatementResponse>> HandleAsync(TransactionStatementRequest request, CancellationToken cancellationToken)
    {
        var validationResult = _validator.TryValidate(request);
        if (validationResult.IsValid is false)
            return RequestHandlerResult<TransactionStatementResponse>.Unprocessable(failureDetails: validationResult.FailureDetails);

        var transactions = await _repository.FindBy(request.StoreName, request.StartDate, request.EndDate, cancellationToken);

        if (transactions is null)
            return RequestHandlerResult<TransactionStatementResponse>.NoContent();

        var accumulatedValue = transactions.Sum(x => x.TransactionValue);

        var response = new TransactionStatementResponse(request.StartDate, request.EndDate, transactions.Count(), accumulatedValue, transactions);

        return RequestHandlerResult<TransactionStatementResponse>.Success(response);

    }
}