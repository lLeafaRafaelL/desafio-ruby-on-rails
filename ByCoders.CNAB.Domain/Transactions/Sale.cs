namespace ByCoders.CNAB.Domain.Transactions;

public sealed class Sale : Transaction
{
    public Sale(DateOnly transactionDate, TimeOnly transactionTimeUtc, decimal amount, Beneficiary beneficiary, Card card, Store store)
        : base(TransactionTypes.Sales, transactionDate, transactionTimeUtc, amount, beneficiary, card, store)
    {
    }

}
