namespace ByCoders.CNAB.Domain.Transactions;

public sealed class Credit : Transaction
{
    public Credit(DateOnly transactionDate, TimeOnly transactionTimeUtc, decimal amount, Beneficiary beneficiary, Card card, Store store)
        : base(TransactionTypes.Credit, transactionDate, transactionTimeUtc, amount, beneficiary, card, store)
    {
    }

}
