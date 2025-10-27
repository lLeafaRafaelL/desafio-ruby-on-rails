using ByCoders.CNAB.Domain.Transactions.Models;
using FizzWare.NBuilder;

namespace ByCoders.CNAB.UnitTests.Builders.Domain;

public class TransactionBuilder<T> where T : Transaction
{
    private DateOnly _transactionDate = new(2019, 03, 01);
    private TimeOnly _transactionTime = new(15, 30, 45);
    private decimal _amountCNAB = 14200m;
    private Beneficiary _beneficiary;
    private Card _card;
    private Store _store;
    private Guid _cnabFileId = Guid.NewGuid();

    public TransactionBuilder()
    {
        _beneficiary = BeneficiaryBuilder.New.WithValidCPF();
        _card = CardBuilder.New.WithMaskedNumber();
        _store = StoreBuilder.New.WithDefaultTestData();
    }

    public static TransactionBuilder<T> New => new();

    public TransactionBuilder<T> WithDate(DateOnly date)
    {
        _transactionDate = date;
        return this;
    }

    public TransactionBuilder<T> WithTime(TimeOnly time)
    {
        _transactionTime = time;
        return this;
    }

    public TransactionBuilder<T> WithAmountCNAB(decimal amount)
    {
        _amountCNAB = amount;
        return this;
    }

    public TransactionBuilder<T> WithBeneficiary(Beneficiary beneficiary)
    {
        _beneficiary = beneficiary;
        return this;
    }

    public TransactionBuilder<T> WithCard(Card card)
    {
        _card = card;
        return this;
    }

    public TransactionBuilder<T> WithStore(Store store)
    {
        _store = store;
        return this;
    }

    public TransactionBuilder<T> WithCNABFileId(Guid cnabFileId)
    {
        _cnabFileId = cnabFileId;
        return this;
    }

    public TransactionBuilder<T> WithRandomData()
    {
        var random = new Random();
        _transactionDate = DateOnly.FromDateTime(DateTime.Now.AddDays(-random.Next(1, 365)));
        _transactionTime = new TimeOnly(random.Next(0, 23), random.Next(0, 59), random.Next(0, 59));
        _amountCNAB = random.Next(100, 999999);
        _beneficiary = BeneficiaryBuilder.New.WithRandomCPF();
        _card = CardBuilder.New.WithRandomMaskedNumber();
        _store = StoreBuilder.New.WithRandomData();
        return this;
    }

    public TransactionBuilder<T> WithZeroAmount()
    {
        _amountCNAB = 0;
        return this;
    }

    public TransactionBuilder<T> WithLargeAmount()
    {
        _amountCNAB = 9999999999m;
        return this;
    }

    public T Build()
    {
        var transaction = (T)Activator.CreateInstance(
            typeof(T), 
            _cnabFileId,
            _transactionDate, 
            _transactionTime, 
            _amountCNAB, 
            _beneficiary, 
            _card, 
            _store)!;
        
        return transaction;
    }

    public static implicit operator T(TransactionBuilder<T> builder)
    {
        return builder.Build();
    }
}

// Specific builders for each transaction type for convenience
public class SaleBuilder : TransactionBuilder<Sale>
{
    public new static SaleBuilder New => new();
}

public class DebitBuilder : TransactionBuilder<Debit>
{
    public new static DebitBuilder New => new();
}

public class CreditBuilder : TransactionBuilder<Credit>
{
    public new static CreditBuilder New => new();
}

public class BankSlipBuilder : TransactionBuilder<BankSlip>
{
    public new static BankSlipBuilder New => new();
}

public class FundingBuilder : TransactionBuilder<Funding>
{
    public new static FundingBuilder New => new();
}

public class LoanReceiptBuilder : TransactionBuilder<LoanReceipt>
{
    public new static LoanReceiptBuilder New => new();
}

public class TEDReceiptBuilder : TransactionBuilder<TEDReceipt>
{
    public new static TEDReceiptBuilder New => new();
}

public class DOCReceiptBuilder : TransactionBuilder<DOCReceipt>
{
    public new static DOCReceiptBuilder New => new();
}

public class RentBuilder : TransactionBuilder<Rent>
{
    public new static RentBuilder New => new();
}
