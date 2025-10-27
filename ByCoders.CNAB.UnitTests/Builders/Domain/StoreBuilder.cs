using ByCoders.CNAB.Domain.Transactions.Models;
using FizzWare.NBuilder;

namespace ByCoders.CNAB.UnitTests.Builders.Domain;

public class StoreBuilder
{
    private string _name = "Test Store";
    private string _owner = "John Doe";

    public static StoreBuilder New => new();

    public StoreBuilder WithName(string name)
    {
        _name = name;
        return this;
    }

    public StoreBuilder WithOwner(string owner)
    {
        _owner = owner;
        return this;
    }

    public StoreBuilder WithDefaultTestData()
    {
        _name = "BAR DO JOÃO";
        _owner = "JOÃO MACEDO";
        return this;
    }

    public StoreBuilder WithRandomData()
    {
        var stores = new[]
        {
            ("MERCADO DA AVENIDA", "FERNANDA COSTA"),
            ("LOJA DO Ó - MATRIZ", "JOSÉ COSTA"),
            ("MERCEARIA 3 IRMÃOS", "MARCOS PEREIRA"),
            ("BAR DO JOÃO", "JOÃO MACEDO"),
            ("PADARIA TRÊS IRMÃS", "MARIA SILVA")
        };
        
        var random = new Random();
        var selected = stores[random.Next(stores.Length)];
        _name = selected.Item1;
        _owner = selected.Item2;
        return this;
    }

    public Store Build()
    {
        return new Store(_name, _owner);
    }

    public static implicit operator Store(StoreBuilder builder)
    {
        return builder.Build();
    }
}
