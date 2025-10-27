namespace ByCoders.CNAB.Domain.Transactions.Models;

/// <summary>
/// Funding - Type 3
/// Nature: Cash Out (-)
/// </summary>
public class Funding : Transaction
{
    public Funding(Guid cnabFileId, DateOnly transactionDate, TimeOnly transactionTimeUtc, decimal amount, Beneficiary beneficiary, Card card, Store store)
        : base(TransactionTypes.Funding, cnabFileId, transactionDate, transactionTimeUtc, amount, beneficiary, card, store)
    {
    }

    public override decimal TransactionValue => -AmountCNAB / 100;
}
