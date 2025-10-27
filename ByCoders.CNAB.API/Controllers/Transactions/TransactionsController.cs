using ByCoders.CNAB.Domain.Transactions;
using Microsoft.AspNetCore.Mvc;

namespace ByCoders.CNAB.API.Controllers.Transactions;

/// <summary>
/// Controller to get cnab transactions
/// Agregado: Transaction
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public class TransactionsController : ControllerBase
{
    private readonly ILogger<TransactionsController> _logger;

    public TransactionsController(
        ILogger<TransactionsController> logger)
    {
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



        return Ok();
    }
}
