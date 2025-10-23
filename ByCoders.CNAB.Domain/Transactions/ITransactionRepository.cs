using ByCoders.CNAB.Domain.Transactions.Models;

namespace ByCoders.CNAB.Domain.Transactions;

/// <summary>
/// Repository interface for Transaction entity
/// </summary>
public interface ITransactionRepository
{

    /// <summary>
    /// Bulk insert multiple transactions (high performance)
    /// </summary>
    Task BulkInsertAsync(IEnumerable<Transaction> transactions, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets transactions by store
    /// </summary>
    Task<IEnumerable<Transaction>> GetByStoreAsync(string storeName, CancellationToken cancellationToken = default);

    /// <summary>
    /// Saves changes to the database
    /// </summary>
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}
