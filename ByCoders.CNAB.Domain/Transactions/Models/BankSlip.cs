namespace ByCoders.CNAB.Domain.Transactions.Models;

public class BankSlip : Transaction
{
    public BankSlip(DateOnly transactionDate, TimeOnly transactionTimeUtc, decimal amount, Beneficiary beneficiary, Card card, Store store)
        : base(TransactionTypes.BankSlip, transactionDate, transactionTimeUtc, amount, beneficiary, card, store)
    {
    }

    public override decimal TransactionValue => -AmountCNAB / 100;
}
