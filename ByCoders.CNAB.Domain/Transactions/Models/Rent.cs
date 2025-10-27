namespace ByCoders.CNAB.Domain.Transactions.Models;

/// <summary>
/// Rent - Type 9
/// Nature: Casu Out (-)
/// </summary>
public class Rent : Transaction
{
    public Rent(Guid cnabFileId, DateOnly transactionDate, TimeOnly transactionTimeUtc, decimal amount, Beneficiary beneficiary, Card card, Store store)
        : base(TransactionTypes.Rent, cnabFileId, transactionDate, transactionTimeUtc, amount, beneficiary, card, store)
    {
    }

    public override decimal TransactionValue => - AmountCNAB / 100;
}
