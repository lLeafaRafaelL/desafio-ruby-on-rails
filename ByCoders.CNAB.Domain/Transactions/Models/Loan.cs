namespace ByCoders.CNAB.Domain.Transactions.Models;

/// <summary>
/// Recebimento Empréstimo - Tipo 5
/// Natureza: ENTRADA (+)
/// Conforme README.md do projeto
/// </summary>
public class LoanReceipt : Transaction
{
    public LoanReceipt(DateOnly transactionDate, TimeOnly transactionTimeUtc, decimal amount, Beneficiary beneficiary, Card card, Store store)
        : base(TransactionTypes.LoanReceipt, transactionDate, transactionTimeUtc, amount, beneficiary, card, store)
    {
    }
}
