using ByCoders.CNAB.Domain.Transactions;
using ByCoders.CNAB.Domain.Transactions.Models;
using ByCoders.CNAB.Infrastructure.EntityFrameworkCore.Configurations;
using Microsoft.EntityFrameworkCore;
using EFCore.BulkExtensions;

namespace ByCoders.CNAB.Infrastructure.Repositories;

/// <summary>
/// Repository implementation for Transaction entity using Entity Framework Core
/// </summary>
public class TransactionRepository : ITransactionRepository
{
    private readonly TransactionDbContext _context;

    public TransactionRepository(TransactionDbContext context)
    {
        _context = context;
    }

    public async Task AddAsync(Transaction transaction, CancellationToken cancellationToken = default)
    {
        await _context.Transactions.AddAsync(transaction, cancellationToken);
    }

    public async Task AddRangeAsync(IEnumerable<Transaction> transactions, CancellationToken cancellationToken = default)
    {
        await _context.Transactions.AddRangeAsync(transactions, cancellationToken);
    }

    public async Task BulkInsertAsync(IEnumerable<Transaction> transactions, CancellationToken cancellationToken = default)
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

    public async Task<Transaction?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _context.Transactions
            .Include(t => t.TransactionType)
            .FirstOrDefaultAsync(t => t.Id == id, cancellationToken);
    }

    public async Task<IEnumerable<Transaction>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await _context.Transactions
            .Include(t => t.TransactionType)
            .OrderByDescending(t => t.TransactionDate)
            .ThenByDescending(t => t.TransactionTime)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<Transaction>> GetByStoreAsync(string storeName, CancellationToken cancellationToken = default)
    {
        return await _context.Transactions
            .Include(t => t.TransactionType)
            .Where(t => t.Store.Name == storeName)
            .OrderByDescending(t => t.TransactionDate)
            .ThenByDescending(t => t.TransactionTime)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<Transaction>> GetByDateRangeAsync(DateOnly startDate, DateOnly endDate, CancellationToken cancellationToken = default)
    {
        return await _context.Transactions
            .Include(t => t.TransactionType)
            .Where(t => t.TransactionDate >= startDate && t.TransactionDate <= endDate)
            .OrderByDescending(t => t.TransactionDate)
            .ThenByDescending(t => t.TransactionTime)
            .ToListAsync(cancellationToken);
    }

    public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        return await _context.SaveChangesAsync(cancellationToken);
    }
}
