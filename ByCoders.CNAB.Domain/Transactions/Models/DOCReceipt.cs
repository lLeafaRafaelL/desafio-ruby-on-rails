namespace ByCoders.CNAB.Domain.Transactions.Models;

/// <summary>
/// Recebimento DOC - Tipo 8
/// Natureza: ENTRADA (+)
/// Conforme README.md do projeto
/// </summary>
public class DOCReceipt : Transaction
{
    public DOCReceipt(DateOnly transactionDate, TimeOnly transactionTimeUtc, decimal amount, Beneficiary beneficiary, Card card, Store store)
        : base(TransactionTypes.DOCReceipt, transactionDate, transactionTimeUtc, amount, beneficiary, card, store)
    {
    }

}
