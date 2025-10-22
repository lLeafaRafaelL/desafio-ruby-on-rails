namespace ByCoders.CNAB.Domain.Transactions;

public class Debit : Transaction
{
    public Debit( DateOnly transactionDate, TimeOnly transactionTimeUtc, decimal amount, Beneficiary beneficiary, Card card, Store store)
        : base(TransactionTypes.Debit, transactionDate, transactionTimeUtc, amount, beneficiary, card, store)
    {

    }

    public override decimal TransactionValue => -AmountCNAB / 100;
}
