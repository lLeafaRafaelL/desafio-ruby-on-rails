using ByCoders.CNAB.Domain.Transactions.Models;
using ByCoders.CNAB.Infrastructure.EntityFrameworkCore.Configurations;
using Microsoft.EntityFrameworkCore;
using EFCore.BulkExtensions;
using ByCoders.CNAB.Domain.Transactions;
using ByCoders.CNAB.Infrastructure.EntityFrameworkCore;
using System.Linq.Expressions;

namespace ByCoders.CNAB.Infrastructure.Repositories;

public class TransactionRepository : ITransactionRepository
{
    private readonly CNABFileDbContext _context;

    public TransactionRepository(CNABFileDbContext context)
    {
        _context = context;
    }

    public async Task BulkInsertAsync(IEnumerable<Transaction> transactions, CancellationToken cancellationToken)
    {
        var transactionsList = transactions.ToList();
        if (!transactionsList.Any())
            return;

        // Use database transaction to ensure atomicity
        await using var dbTransaction = await _context.Database.BeginTransactionAsync(cancellationToken);
        try
        {
            // BulkInsert configuration optimized for Transaction entity
            // Docs: https://github.com/borisdj/EFCore.BulkExtensions
            var bulkConfig = new BulkConfig
            {
                SetOutputIdentity = false,      // We use Guid.NewGuid() in constructor
                PreserveInsertOrder = false,    // Better performance when order doesn't matter
                BatchSize = 2000,               // Optimal batch size (default is 2000)
                BulkCopyTimeout = 300           // 5 minutes timeout for large imports
            };

            // Bulk insert using EFCore.BulkExtensions
            await _context.BulkInsertAsync(transactionsList, bulkConfig, null, null, cancellationToken);
            
            // Commit transaction
            await dbTransaction.CommitAsync(cancellationToken);
        }
        catch
        {
            // Rollback on error
            await dbTransaction.RollbackAsync(cancellationToken);
            throw;
        }
    }

    public async Task<IEnumerable<Transaction>> GetByDateRangeAsync(DateOnly startDate, DateOnly endDate, CancellationToken cancellationToken)
    {
        return await _context.Transactions
            .Where(t => t.TransactionDate >= startDate && t.TransactionDate <= endDate)
            .OrderByDescending(t => t.TransactionDate)
            .ThenByDescending(t => t.TransactionTime)
            .ToListAsync(cancellationToken);
    }

    public async Task<int> SaveChangesAsync(CancellationToken cancellationToken)
    {
        return await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task<IEnumerable<Transaction>> FindBy(Expression<Func<Transaction, bool>> predicate, CancellationToken cancellationToken, bool splitQuery = true, bool asNoTracking = false)
    {
        var query = _context.Transactions.Where(predicate);

        if (asNoTracking)
            query = query.AsNoTracking();

        if (splitQuery)
            query = query.AsSplitQuery();

        return await query.ToListAsync();
    }

    public async Task<IEnumerable<Transaction>> FindBy(string storeName, DateTimeOffset startDate, DateTimeOffset endDate, CancellationToken cancellationToken)
    {
        return await _context.Transactions
            .AsSplitQuery() // Optimize performance
            .AsNoTracking() // Optimize performance
            .Where(t => t.Store.Name.Equals(storeName, StringComparison.InvariantCultureIgnoreCase) && t.TransactionDateTime.Date >= startDate && t.TransactionDateTime <= endDate)
            .ToListAsync(cancellationToken);
    }
}
