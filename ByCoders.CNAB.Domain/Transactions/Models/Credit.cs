namespace ByCoders.CNAB.Domain.Transactions.Models;

/// <summary>
/// Crédito - Tipo 4
/// Natureza: ENTRADA (+)
/// Conforme README.md do projeto
/// </summary>
public class Credit : Transaction
{
    public Credit(DateOnly transactionDate, TimeOnly transactionTimeUtc, decimal amount, Beneficiary beneficiary, Card card, Store store)
        : base(TransactionTypes.Credit, transactionDate, transactionTimeUtc, amount, beneficiary, card, store)
    {
    }

}
