namespace ByCoders.CNAB.Domain.Transactions.Models;

/// <summary>
/// Aluguel - Tipo 9
/// Natureza: SAÍDA (-)
/// Conforme README.md do projeto
/// </summary>
public class Rent : Transaction
{
    public Rent(DateOnly transactionDate, TimeOnly transactionTimeUtc, decimal amount, Beneficiary beneficiary, Card card, Store store)
        : base(TransactionTypes.Rent, transactionDate, transactionTimeUtc, amount, beneficiary, card, store)
    {
    }

    public override decimal TransactionValue => -AmountCNAB / 100;
}
