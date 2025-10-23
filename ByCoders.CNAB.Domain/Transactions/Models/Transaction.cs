using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ByCoders.CNAB.Domain.Transactions.Models;

public abstract class Transaction
{
    protected Transaction()
    {
        Id = Guid.NewGuid();
        CreatedOn = DateTimeOffset.UtcNow;
    }

    protected Transaction(
        TransactionTypes transactionType,
        DateOnly transactionDate,
        TimeOnly transactionTimeUtc,
        decimal amountCNAB,
        Beneficiary beneficiary,
        Card card,
        Store store) : this()
    {
        TransactionDate = transactionDate;
        TransactionTime = transactionTimeUtc;
        TransactionType = new TransactionType(transactionType);
        AmountCNAB = amountCNAB;
        Beneficiary = beneficiary;
        Card = card;
        Store = store;
    }

    public TransactionType TransactionType { get; protected set; }
    public Guid Id { get; protected set; }
    public DateTimeOffset CreatedOn { get; init; }
    public DateOnly TransactionDate { get; protected set; }
    public TimeOnly TransactionTime { get; protected set; }
    public decimal AmountCNAB { get; protected set; }
    public virtual decimal TransactionValue => AmountCNAB / 100;

    public Beneficiary Beneficiary { get; protected set; }
    public Card Card { get; protected set; }
    public Store Store { get; protected set; }
}

public record Beneficiary(string Document);

public record Card(string Number);

public record Store(string Name, string Owner);