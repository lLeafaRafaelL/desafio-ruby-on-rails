namespace ByCoders.CNAB.Domain.Transactions.Models;

/// <summary>
/// Ted Receipt  - Type 7
/// Nature: Cash In (+)
/// </summary>
public class TEDReceipt : Transaction
{
    public TEDReceipt(Guid cnabFileId, DateOnly transactionDate, TimeOnly transactionTimeUtc, decimal amount, Beneficiary beneficiary, Card card, Store store)
        : base(TransactionTypes.TEDReceipt, cnabFileId, transactionDate, transactionTimeUtc, amount, beneficiary, card, store)
    {
    }

}
