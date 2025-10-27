using ByCoders.CNAB.Domain.Transactions.Models;
using FizzWare.NBuilder;

namespace ByCoders.CNAB.UnitTests.Builders.Domain;

public class CardBuilder
{
    private string _number = "1234****5678";

    public static CardBuilder New => new();

    public CardBuilder WithNumber(string number)
    {
        _number = number;
        return this;
    }

    public CardBuilder WithMaskedNumber()
    {
        _number = "4753****3153"; // From test data
        return this;
    }

    public CardBuilder WithRandomMaskedNumber()
    {
        var random = new Random();
        var firstFour = random.Next(1000, 9999);
        var lastFour = random.Next(1000, 9999);
        _number = $"{firstFour}****{lastFour}";
        return this;
    }

    public Card Build()
    {
        return new Card(_number);
    }

    public static implicit operator Card(CardBuilder builder)
    {
        return builder.Build();
    }
}
