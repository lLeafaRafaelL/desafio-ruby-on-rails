using ByCoders.CNAB.Application.Transactions.FindTransactions;
using ByCoders.CNAB.Core;
using ByCoders.CNAB.Core.Http;
using ByCoders.CNAB.Core.Results;
using Microsoft.AspNetCore.Mvc;

namespace ByCoders.CNAB.API.Controllers.Transactions;

/// <summary>
/// Controller to get cnab transactions
/// Aggregate Root: Transaction
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public class TransactionsController : BaseController
{
    private readonly IRequestHandler<TransactionStatementRequest, TransactionStatementResponse> _handler;

    public TransactionsController(
        IRequestHandler<TransactionStatementRequest, TransactionStatementResponse> handler,
        ILogger<TransactionsController> logger) : base (logger)
    {
        _handler = handler;
    }

    /// <summary>
    /// Busca transações por loja
    /// </summary>
    /// <param name="storeName">Nome da loja</param>
    /// <param name="cancellationToken">Token de cancelamento</param>
    /// <returns>Store Transaction</returns>
    /// <response code="200">Transações encontradas</response>
    [HttpGet("store/{storeName}")]
    [ProducesResponseType(typeof(TransactionStatementResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(TransactionStatementResponse), StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ResultFailureDetail), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> GetByStore(
        [FromRoute] string storeName, 
        [FromQuery] DateTimeOffset fromDate, 
        [FromQuery] DateTimeOffset toDate, 
        CancellationToken cancellationToken)
    {
        _logger.LogInformation("Searching for transactions for store: {StoreName} {FromDate} {ToDate}", storeName, fromDate, toDate);

        var request = new TransactionStatementRequest(storeName, fromDate, toDate);

        var result = await _handler.HandleAsync(request, cancellationToken);

        return ResponseToActionResult(result, _ => result.Value, _ => result.FailureDetails);
    }
}
