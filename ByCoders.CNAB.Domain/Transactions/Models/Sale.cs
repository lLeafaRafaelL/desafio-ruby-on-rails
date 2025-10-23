namespace ByCoders.CNAB.Domain.Transactions.Models;

/// <summary>
/// Vendas - Tipo 6
/// Natureza: ENTRADA (+)
/// Conforme README.md do projeto
/// </summary>
public class Sale : Transaction
{
    public Sale(DateOnly transactionDate, TimeOnly transactionTimeUtc, decimal amount, Beneficiary beneficiary, Card card, Store store)
        : base(TransactionTypes.Sales, transactionDate, transactionTimeUtc, amount, beneficiary, card, store)
    {
    }

}
