using ByCoders.CNAB.Domain.Transactions.Models;
using FizzWare.NBuilder;

namespace ByCoders.CNAB.UnitTests.Builders.Domain;

public class BeneficiaryBuilder
{
    private string _document = "12345678901";

    public static BeneficiaryBuilder New => new();

    public BeneficiaryBuilder WithDocument(string document)
    {
        _document = document;
        return this;
    }

    public BeneficiaryBuilder WithValidCPF()
    {
        _document = "09620676017"; // Valid CPF from test data
        return this;
    }

    public BeneficiaryBuilder WithRandomCPF()
    {
        _document = Builder<string>
            .CreateNew()
            .With(x => x = GenerateRandomCPF())
            .Build();
        return this;
    }

    public Beneficiary Build()
    {
        return new Beneficiary(_document);
    }

    public static implicit operator Beneficiary(BeneficiaryBuilder builder)
    {
        return builder.Build();
    }

    private static string GenerateRandomCPF()
    {
        var random = new Random();
        return random.NextInt64(10000000000, 99999999999).ToString("D11");
    }
}
