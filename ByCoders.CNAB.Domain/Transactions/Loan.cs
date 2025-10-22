namespace ByCoders.CNAB.Domain.Transactions;

public class LoanReceipt : Transaction
{
    public LoanReceipt(DateOnly transactionDate, TimeOnly transactionTimeUtc, decimal amount, Beneficiary beneficiary, Card card, Store store)
        : base(TransactionTypes.LoanReceipt, transactionDate, transactionTimeUtc, amount, beneficiary, card, store)
    {
    }
}
