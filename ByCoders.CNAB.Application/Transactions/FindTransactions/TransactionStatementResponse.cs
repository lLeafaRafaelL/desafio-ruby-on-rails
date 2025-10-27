using ByCoders.CNAB.Core;
using ByCoders.CNAB.Domain.Transactions.Models;

namespace ByCoders.CNAB.Application.Transactions.FindTransactions;

public record TransactionStatementRequest(string StoreName, DateTimeOffset StartDate, DateTimeOffset EndDate) : Dto;

public record TransactionStatementResponse : Dto
{
    public TransactionStatementResponse(DateTimeOffset startDate, DateTimeOffset endDate, int totalTrsanctions, decimal accumulatedValue, IEnumerable<Transaction> transactions)
    {
        StartDate = startDate;
        EndDate = endDate;
        TotalTrsanctions = totalTrsanctions;
        AccumulatedValue = accumulatedValue;
        Transactions = transactions;
    }

    public DateTimeOffset StartDate { get; set; }
    public DateTimeOffset EndDate { get; set; }
    public int TotalTrsanctions { get; set; }
    public decimal AccumulatedValue { get; set; }
    public IEnumerable<Transaction> Transactions { get; set; } = Enumerable.Empty<Transaction>();
}
