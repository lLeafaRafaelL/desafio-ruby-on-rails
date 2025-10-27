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
    Task BulkInsertAsync(IEnumerable<Transaction> transactions, CancellationToken cancellationToken);

    /// <summary>
    /// Gets transactions by store
    /// </summary>
    Task<IEnumerable<Transaction>> GetByStoreAsync(string storeName, CancellationToken cancellationToken);

    /// <summary>
    /// Gets transactions by CNAB file ID
    /// </summary>
    Task<IEnumerable<Transaction>> GetByCNABFileIdAsync(Guid cnabFileId, CancellationToken cancellationToken);

    /// <summary>
    /// Saves changes to the database
    /// </summary>
    Task<int> SaveChangesAsync(CancellationToken cancellationToken);
}
