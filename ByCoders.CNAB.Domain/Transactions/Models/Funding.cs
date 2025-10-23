namespace ByCoders.CNAB.Domain.Transactions.Models;

/// <summary>
/// Financiamento - Tipo 3
/// Natureza: SAÍDA (-)
/// Conforme README.md do projeto
/// </summary>
public class Funding : Transaction
{
    public Funding(DateOnly transactionDate, TimeOnly transactionTimeUtc, decimal amount, Beneficiary beneficiary, Card card, Store store)
        : base(TransactionTypes.Funding, transactionDate, transactionTimeUtc, amount, beneficiary, card, store)
    {
    }

    public override decimal TransactionValue => -AmountCNAB / 100;
}
