namespace ByCoders.CNAB.Domain.Transactions.Models;

/// <summary>
/// BankSlip - Type 2
/// Nature: Cash Out (-)
/// </summary>
public class BankSlip : Transaction
{
    private BankSlip() { }

    public BankSlip(Guid cnabFileId, DateOnly transactionDate, TimeOnly transactionTimeUtc, decimal amount, Beneficiary beneficiary, Card card, Store store)
        : base(TransactionTypes.BankSlip, cnabFileId, transactionDate, transactionTimeUtc, amount, beneficiary, card, store)
    {
    }

    public override decimal TransactionValue => -AmountCNAB / 100;
}
