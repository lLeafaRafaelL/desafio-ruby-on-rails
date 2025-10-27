using ByCoders.CNAB.Core.Results;
using ByCoders.CNAB.Domain.Transactions.Models;

namespace ByCoders.CNAB.Application.Transactions;

/// <summary>
/// Factory interface for creating Transaction instances based on CNAB data
/// </summary>
public interface ITransactionFactory
{
    /// <summary>
    /// Creates a Transaction instance from CNAB line data
    /// </summary>
    /// <param name="data">Parsed CNAB line data</param>
    /// <returns>Result containing the created Transaction or an error message</returns>
    Result<Transaction> Create(Guid cnabFileId, CNABFactoryParams data);
}
