using ByCoders.CNAB.Domain.Transactions;
using Microsoft.AspNetCore.Mvc;

namespace ByCoders.CNAB.API.Controllers.Transactions;

/// <summary>
/// Controller para consulta de transações CNAB
/// Agregado: Transaction
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public class TransactionsController : ControllerBase
{
    private readonly ITransactionRepository _transactionRepository;
    private readonly ILogger<TransactionsController> _logger;

    public TransactionsController(
        ITransactionRepository transactionRepository,
        ILogger<TransactionsController> logger)
    {
        _transactionRepository = transactionRepository;
        _logger = logger;
    }

    /// <summary>
    /// Busca transações por loja
    /// </summary>
    /// <param name="storeName">Nome da loja</param>
    /// <param name="cancellationToken">Token de cancelamento</param>
    /// <returns>Lista de transações da loja</returns>
    /// <response code="200">Transações encontradas</response>
    [HttpGet("store/{storeName}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> GetByStore(string storeName, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Buscando transações para loja: {StoreName}", storeName);

        var transactions = await _transactionRepository.GetByStoreAsync(storeName, cancellationToken);
        var transactionsList = transactions.ToList();

        var result = new
        {
            storeName,
            totalTransactions = transactionsList.Count,
            cashIn = transactionsList.Where(t => t.TransactionValue > 0).Sum(t => t.TransactionValue),
            cashOut = transactionsList.Where(t => t.TransactionValue < 0).Sum(t => t.TransactionValue),
            balance = transactionsList.Sum(t => t.TransactionValue),
            transactions = transactionsList.Select(t => new
            {
                id = t.Id,
                type = t.TransactionType.ToString(),
                nature = t.TransactionType.Nature.ToString(),
                date = t.TransactionDate,
                time = t.TransactionTime,
                value = t.TransactionValue,
                beneficiary = t.Beneficiary.Document,
                card = t.Card.Number
            })
        };

        return Ok(result);
    }
}
