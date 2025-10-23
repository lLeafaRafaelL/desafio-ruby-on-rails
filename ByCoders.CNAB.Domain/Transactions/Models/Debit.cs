namespace ByCoders.CNAB.Domain.Transactions.Models;

/// <summary>
/// Débito - Tipo 1
/// Natureza: ENTRADA (+)
/// Conforme README.md do projeto
/// </summary>
public class Debit : Transaction
{
    public Debit(DateOnly transactionDate, TimeOnly transactionTimeUtc, decimal amount, Beneficiary beneficiary, Card card, Store store)
        : base(TransactionTypes.Debit, transactionDate, transactionTimeUtc, amount, beneficiary, card, store)
    {
    }
    
    // Debit é ENTRADA, usa o padrão positivo da classe base (AmountCNAB / 100)
}
