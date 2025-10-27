namespace ByCoders.CNAB.Domain.Transactions.Models;

/// <summary>
/// DOC Receipt - Type 8
/// Nature: Cash In (+)
/// </summary>
public class DOCReceipt : Transaction
{
    private DOCReceipt() { }

    public DOCReceipt(Guid cnabFileId, DateOnly transactionDate, TimeOnly transactionTimeUtc, decimal amount, Beneficiary beneficiary, Card card, Store store)
        : base(cnabFileId, transactionDate, transactionTimeUtc, amount, beneficiary, card, store)
    {
    }

}
