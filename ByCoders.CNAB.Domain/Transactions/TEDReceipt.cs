namespace ByCoders.CNAB.Domain.Transactions;

public sealed class TEDReceipt : Transaction
{
    public TEDReceipt(DateOnly transactionDate, TimeOnly transactionTimeUtc, decimal amount, Beneficiary beneficiary, Card card, Store store)
        : base(TransactionTypes.TEDReceipt, transactionDate, transactionTimeUtc, amount, beneficiary, card, store)
    {
    }

}
