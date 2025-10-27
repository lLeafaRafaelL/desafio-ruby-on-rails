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
            // Configure discriminator shadow property for each transaction
            // BulkInsert doesn't automatically set TPH discriminators
            foreach (var transaction in transactionsList)
            {
                var discriminatorValue = transaction switch
                {
                    Debit => (int)TransactionTypes.Debit,
                    Credit => (int)TransactionTypes.Credit,
                    BankSlip => (int)TransactionTypes.BankSlip,
                    Funding => (int)TransactionTypes.Funding,
                    LoanReceipt => (int)TransactionTypes.LoanReceipt,
                    Sale => (int)TransactionTypes.Sales,
                    TEDReceipt => (int)TransactionTypes.TEDReceipt,
                    DOCReceipt => (int)TransactionTypes.DOCReceipt,
                    Rent => (int)TransactionTypes.Rent,
                    _ => throw new InvalidOperationException($"Unknown transaction type: {transaction.GetType().Name}")
                };

                // Set shadow property using EF Core's entry
                _context.Entry(transaction).Property("TransactionTypeId").CurrentValue = discriminatorValue;
            }

            // BulkInsert configuration optimized for Transaction entity
            var bulkConfig = new BulkConfig
            {
                SetOutputIdentity = false,
                PreserveInsertOrder = false,
                BatchSize = 2000,
                BulkCopyTimeout = 300
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
            .AsNoTracking() // Optimize performance
            .Where(t => t.TransactionDateTime >= startDate.UtcDateTime && t.TransactionDateTime <= endDate.UtcDateTime)
            .Where(t => t.Store.Name.Contains(storeName))   
            .ToListAsync(cancellationToken);
    }
}
