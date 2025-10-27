namespace ByCoders.CNAB.Domain.Transactions.Models;

/// <summary>
/// Debit - Type 1
/// Nature: Cash In (+)
/// </summary>
public class Debit : Transaction
{
    private Debit() { }

    public Debit(Guid cnabFileId, DateOnly transactionDate, TimeOnly transactionTimeUtc, decimal amount, Beneficiary beneficiary, Card card, Store store)
        : base(TransactionTypes.Debit, cnabFileId, transactionDate, transactionTimeUtc, amount, beneficiary, card, store)
    {
    }    
}
