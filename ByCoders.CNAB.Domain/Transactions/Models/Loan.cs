namespace ByCoders.CNAB.Domain.Transactions.Models;

/// <summary>
/// Loan Receipt - Type 5
/// Nature: Cash In (+)
/// </summary>
public class LoanReceipt : Transaction
{
    public LoanReceipt(Guid cnabFileId, DateOnly transactionDate, TimeOnly transactionTimeUtc, decimal amount, Beneficiary beneficiary, Card card, Store store)
        : base(TransactionTypes.LoanReceipt, cnabFileId, transactionDate, transactionTimeUtc, amount, beneficiary, card, store)
    {
    }
}
