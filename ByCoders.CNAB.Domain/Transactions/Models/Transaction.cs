using ByCoders.CNAB.Domain.Files.Models;
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
        Id = Guid.CreateVersion7();
        CreatedOn = DateTimeOffset.UtcNow;
    }

    protected Transaction(
        TransactionTypes transactionType,
        Guid cnabFileId,
        DateOnly transactionDate,
        TimeOnly transactionTimeUtc,
        decimal amountCNAB,
        Beneficiary beneficiary,
        Card card,
        Store store) : this()
    {
        CNABFileId = cnabFileId;
        TransactionDateTime = new DateTimeOffset(transactionDate.Year, transactionDate.Month, transactionDate.Day, transactionTimeUtc.Hour, transactionTimeUtc.Minute, transactionTimeUtc.Second, TimeSpan.Zero);
        TransactionType = new TransactionType(transactionType);
        AmountCNAB = amountCNAB;
        Beneficiary = beneficiary;
        Card = card;
        Store = store;
    }

    // Identity
    public Guid Id { get; protected set; }
    public DateTimeOffset CreatedOn { get; init; }

    // Reference to the CNAB file
    public Guid? CNABFileId { get; internal set; }

    // Transaction data
    public TransactionType TransactionType { get; protected set; }
    public DateTimeOffset TransactionDateTime { get; protected set; }
    public DateOnly TransactionDate => DateOnly.FromDateTime(TransactionDateTime.Date);
    public TimeOnly TransactionTime => TimeOnly.FromTimeSpan(TransactionDateTime.TimeOfDay);
    public decimal AmountCNAB { get; protected set; }
    public virtual decimal TransactionValue => AmountCNAB / 100;

    // Value Objects
    public Beneficiary Beneficiary { get; protected set; }
    public Card Card { get; protected set; }
    public Store Store { get; protected set; }


}

public record Beneficiary(string Document);

public record Card(string Number);

public record Store(string Name, string Owner);