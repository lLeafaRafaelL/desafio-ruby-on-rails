using Azure.Core;
using ByCoders.CNAB.Core;
using ByCoders.CNAB.Domain.Transactions.Models;

namespace ByCoders.CNAB.Application.Transactions.FindTransactions;

public record TransactionStatementResponse : Dto
{
    public TransactionStatementResponse(DateTimeOffset startDate, DateTimeOffset endDate, IEnumerable<Transaction> transactions)
    {
        StartDate = startDate;
        EndDate = endDate;
        Transactions = transactions;
    }

    public DateTimeOffset StartDate { get; set; }
    public DateTimeOffset EndDate { get; set; }

    public IEnumerable<Transaction> Transactions { get; set; } = Enumerable.Empty<Transaction>();
    public decimal AccumulatedValue => Transactions?.Sum(x => x.TransactionValue) ?? 0;
    public int TotalTrsanctions => Transactions?.Count() ?? 0;
}