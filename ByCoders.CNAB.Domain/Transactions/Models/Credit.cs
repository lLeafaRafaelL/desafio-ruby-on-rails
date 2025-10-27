namespace ByCoders.CNAB.Domain.Transactions.Models;

/// <summary>
/// Credit- Type 4
/// Nature: Cash In (+)
/// </summary>
public class Credit : Transaction
{
    public Credit(Guid cnabFileId, DateOnly transactionDate, TimeOnly transactionTimeUtc, decimal amount, Beneficiary beneficiary, Card card, Store store)
        : base(TransactionTypes.Credit, cnabFileId, transactionDate, transactionTimeUtc, amount, beneficiary, card, store)
    {
    }

}
