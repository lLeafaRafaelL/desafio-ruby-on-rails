using ByCoders.CNAB.Domain.Transactions.Models;
using System.Linq.Expressions;

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
    /// Finds transactions by store name and start and end date
    /// </summary>
    /// <param name="storeName"></param>
    /// <param name="startDate"></param>
    /// <param name="endDate"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<IEnumerable<Transaction>> FindBy(string storeName, DateTimeOffset startDate, DateTimeOffset endDate, CancellationToken cancellationToken);

    /// <summary>
    /// Finds transactions by predicate
    /// </summary>
    Task<IEnumerable<Transaction>> FindBy(Expression<Func<Transaction, bool>> predicate, CancellationToken cancellationToken, bool splitQuery = true, bool asNoTracking = false);

    /// <summary>
    /// Saves changes to the database
    /// </summary>
    Task<int> SaveChangesAsync(CancellationToken cancellationToken);
}
