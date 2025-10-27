namespace ByCoders.CNAB.Domain.Transactions.Models;

/// <summary>
/// Sales - Type 6
/// Nature: Cash In (+)
/// </summary>
public class Sale : Transaction
{
    public Sale(Guid cnabFileId, DateOnly transactionDate, TimeOnly transactionTimeUtc, decimal amount, Beneficiary beneficiary, Card card, Store store)
        : base(TransactionTypes.Sales, cnabFileId, transactionDate, transactionTimeUtc, amount, beneficiary, card, store)
    {
    }

}
