namespace ByCoders.CNAB.Domain.Transactions.Models;

public class DOCReceipt : Transaction
{
    public DOCReceipt(DateOnly transactionDate, TimeOnly transactionTimeUtc, decimal amount, Beneficiary beneficiary, Card card, Store store)
        : base(TransactionTypes.DOCReceipt, transactionDate, transactionTimeUtc, amount, beneficiary, card, store)
    {
    }

}
