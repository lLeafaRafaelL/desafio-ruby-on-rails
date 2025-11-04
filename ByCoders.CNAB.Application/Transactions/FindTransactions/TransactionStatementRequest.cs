using ByCoders.CNAB.Core;

namespace ByCoders.CNAB.Application.Transactions.FindTransactions;

public record TransactionStatementRequest(string StoreName, DateTimeOffset StartDate, DateTimeOffset EndDate) : Dto
{
    public string IdempotencyKey => $"{StoreName}-{StartDate}-{EndDate}";
}
