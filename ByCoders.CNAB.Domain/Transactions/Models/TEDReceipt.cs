namespace ByCoders.CNAB.Domain.Transactions.Models;

/// <summary>
/// Recebimento TED - Tipo 7
/// Natureza: ENTRADA (+)
/// Conforme README.md do projeto
/// </summary>
public class TEDReceipt : Transaction
{
    public TEDReceipt(DateOnly transactionDate, TimeOnly transactionTimeUtc, decimal amount, Beneficiary beneficiary, Card card, Store store)
        : base(TransactionTypes.TEDReceipt, transactionDate, transactionTimeUtc, amount, beneficiary, card, store)
    {
    }

}
