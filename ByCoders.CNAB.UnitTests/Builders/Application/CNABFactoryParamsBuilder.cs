using ByCoders.CNAB.Application.Transactions;
using ByCoders.CNAB.Domain.Transactions.Models;
using FizzWare.NBuilder;

namespace ByCoders.CNAB.UnitTests.Builders.Application;

public class CNABFactoryParamsBuilder
{
    private TransactionTypes _transactionType = TransactionTypes.Sales;
    private DateOnly _date = new(2019, 03, 01);
    private decimal _amount = 14200m;
    private string _cpf = "09620676017";
    private string _cardNumber = "4753****3153";
    private TimeOnly _time = new(15, 34, 53);
    private string _storeOwner = "JOÃO MACEDO";
    private string _storeName = "BAR DO JOÃO";

    public static CNABFactoryParamsBuilder New => new();

    public CNABFactoryParamsBuilder WithTransactionType(TransactionTypes transactionType)
    {
        _transactionType = transactionType;
        return this;
    }

    public CNABFactoryParamsBuilder WithDate(DateOnly date)
    {
        _date = date;
        return this;
    }

    public CNABFactoryParamsBuilder WithAmount(decimal amount)
    {
        _amount = amount;
        return this;
    }

    public CNABFactoryParamsBuilder WithCPF(string cpf)
    {
        _cpf = cpf;
        return this;
    }

    public CNABFactoryParamsBuilder WithCardNumber(string cardNumber)
    {
        _cardNumber = cardNumber;
        return this;
    }

    public CNABFactoryParamsBuilder WithTime(TimeOnly time)
    {
        _time = time;
        return this;
    }

    public CNABFactoryParamsBuilder WithStoreOwner(string storeOwner)
    {
        _storeOwner = storeOwner;
        return this;
    }

    public CNABFactoryParamsBuilder WithStoreName(string storeName)
    {
        _storeName = storeName;
        return this;
    }

    public CNABFactoryParamsBuilder WithValidData()
    {
        _transactionType = TransactionTypes.Sales;
        _date = new DateOnly(2019, 03, 01);
        _amount = 14200m;
        _cpf = "09620676017";
        _cardNumber = "4753****3153";
        _time = new TimeOnly(15, 34, 53);
        _storeOwner = "JOÃO MACEDO";
        _storeName = "BAR DO JOÃO";
        return this;
    }

    public CNABFactoryParamsBuilder WithInvalidCPF()
    {
        _cpf = "";
        return this;
    }

    public CNABFactoryParamsBuilder WithInvalidAmount()
    {
        _amount = -100m;
        return this;
    }

    public CNABFactoryParamsBuilder WithEmptyStoreName()
    {
        _storeName = "";
        return this;
    }

    public CNABFactoryParamsBuilder WithEmptyStoreOwner()
    {
        _storeOwner = "";
        return this;
    }

    public CNABFactoryParamsBuilder WithRandomData()
    {
        var random = new Random();
        _transactionType = (TransactionTypes)random.Next(1, 9);
        _date = DateOnly.FromDateTime(DateTime.Now.AddDays(-random.Next(1, 365)));
        _amount = random.Next(100, 999999);
        _cpf = random.NextInt64(10000000000, 99999999999).ToString("D11");
        _cardNumber = $"{random.Next(1000, 9999)}****{random.Next(1000, 9999)}";
        _time = new TimeOnly(random.Next(0, 23), random.Next(0, 59), random.Next(0, 59));
        
        var stores = new[]
        {
            ("MERCADO DA AVENIDA", "FERNANDA COSTA"),
            ("LOJA DO Ó - MATRIZ", "JOSÉ COSTA"),
            ("MERCEARIA 3 IRMÃOS", "MARCOS PEREIRA"),
            ("BAR DO JOÃO", "JOÃO MACEDO"),
            ("PADARIA TRÊS IRMÃS", "MARIA SILVA")
        };
        
        var selected = stores[random.Next(stores.Length)];
        _storeName = selected.Item1;
        _storeOwner = selected.Item2;
        
        return this;
    }

    public CNABFactoryParams Build()
    {
        return new CNABFactoryParams(
            _transactionType, 
            _date, 
            _amount, 
            _cpf, 
            _cardNumber, 
            _time, 
            _storeOwner, 
            _storeName
        );
    }

    public static implicit operator CNABFactoryParams(CNABFactoryParamsBuilder builder)
    {
        return builder.Build();
    }
}
