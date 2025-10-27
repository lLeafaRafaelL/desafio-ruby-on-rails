using ByCoders.CNAB.Domain.Transactions.Models;
using FluentAssertions;
using Xunit;

namespace ByCoders.CNAB.UnitTests.Domain;

public class ValueObjectsTests
{
    [Fact]
    public void Beneficiary_WhenCreated_ShouldStoreDocument()
    {
        // Arrange
        const string expectedDocument = "12345678901";

        // Act
        var beneficiary = new Beneficiary(expectedDocument);

        // Assert
        beneficiary.Document.Should().Be(expectedDocument);
    }

    [Fact]
    public void Beneficiary_WithSameDocument_ShouldBeEqual()
    {
        // Arrange
        const string document = "12345678901";

        // Act
        var beneficiary1 = new Beneficiary(document);
        var beneficiary2 = new Beneficiary(document);

        // Assert
        beneficiary1.Should().Be(beneficiary2);
        beneficiary1.GetHashCode().Should().Be(beneficiary2.GetHashCode());
    }

    [Fact]
    public void Beneficiary_WithDifferentDocument_ShouldNotBeEqual()
    {
        // Arrange
        var beneficiary1 = new Beneficiary("12345678901");
        var beneficiary2 = new Beneficiary("98765432109");

        // Act & Assert
        beneficiary1.Should().NotBe(beneficiary2);
        beneficiary1.GetHashCode().Should().NotBe(beneficiary2.GetHashCode());
    }

    [Fact]
    public void Card_WhenCreated_ShouldStoreNumber()
    {
        // Arrange
        const string expectedNumber = "1234****5678";

        // Act
        var card = new Card(expectedNumber);

        // Assert
        card.Number.Should().Be(expectedNumber);
    }

    [Fact]
    public void Card_WithSameNumber_ShouldBeEqual()
    {
        // Arrange
        const string cardNumber = "1234****5678";

        // Act
        var card1 = new Card(cardNumber);
        var card2 = new Card(cardNumber);

        // Assert
        card1.Should().Be(card2);
        card1.GetHashCode().Should().Be(card2.GetHashCode());
    }

    [Fact]
    public void Card_WithDifferentNumber_ShouldNotBeEqual()
    {
        // Arrange
        var card1 = new Card("1234****5678");
        var card2 = new Card("9876****5432");

        // Act & Assert
        card1.Should().NotBe(card2);
        card1.GetHashCode().Should().NotBe(card2.GetHashCode());
    }

    [Fact]
    public void Store_WhenCreated_ShouldStoreNameAndOwner()
    {
        // Arrange
        const string expectedName = "BAR DO JOÃO";
        const string expectedOwner = "JOÃO MACEDO";

        // Act
        var store = new Store(expectedName, expectedOwner);

        // Assert
        store.Name.Should().Be(expectedName);
        store.Owner.Should().Be(expectedOwner);
    }

    [Fact]
    public void Store_WithSameNameAndOwner_ShouldBeEqual()
    {
        // Arrange
        const string name = "BAR DO JOÃO";
        const string owner = "JOÃO MACEDO";

        // Act
        var store1 = new Store(name, owner);
        var store2 = new Store(name, owner);

        // Assert
        store1.Should().Be(store2);
        store1.GetHashCode().Should().Be(store2.GetHashCode());
    }

    [Fact]
    public void Store_WithDifferentName_ShouldNotBeEqual()
    {
        // Arrange
        var store1 = new Store("BAR DO JOÃO", "JOÃO MACEDO");
        var store2 = new Store("MERCADO CENTRAL", "JOÃO MACEDO");

        // Act & Assert
        store1.Should().NotBe(store2);
    }

    [Fact]
    public void Store_WithDifferentOwner_ShouldNotBeEqual()
    {
        // Arrange
        var store1 = new Store("BAR DO JOÃO", "JOÃO MACEDO");
        var store2 = new Store("BAR DO JOÃO", "MARIA SILVA");

        // Act & Assert
        store1.Should().NotBe(store2);
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData("12345678")]
    [InlineData("123456789012")]
    public void Beneficiary_WithVariousFormats_ShouldAcceptAll(string document)
    {
        // Act
        var act = () => new Beneficiary(document);

        // Assert
        act.Should().NotThrow();
    }

    [Theory]
    [InlineData("1234")]
    [InlineData("1234567890123456")]
    [InlineData("****")]
    [InlineData("XXXX****XXXX")]
    public void Card_WithVariousFormats_ShouldAcceptAll(string number)
    {
        // Act
        var act = () => new Card(number);

        // Assert
        act.Should().NotThrow();
    }

    [Fact]
    public void Store_WithLongNames_ShouldAccept()
    {
        // Arrange
        var longName = new string('A', 100);
        var longOwner = new string('B', 100);

        // Act
        var act = () => new Store(longName, longOwner);

        // Assert
        act.Should().NotThrow();
    }

    [Fact]
    public void Beneficiary_ToString_ShouldReturnDocument()
    {
        // Arrange
        const string document = "12345678901";
        var beneficiary = new Beneficiary(document);

        // Act
        var result = beneficiary.ToString();

        // Assert
        result.Should().Contain(document);
    }

    [Fact]
    public void Card_ToString_ShouldReturnNumber()
    {
        // Arrange
        const string number = "1234****5678";
        var card = new Card(number);

        // Act
        var result = card.ToString();

        // Assert
        result.Should().Contain(number);
    }

    [Fact]
    public void Store_ToString_ShouldContainNameAndOwner()
    {
        // Arrange
        var store = new Store("BAR DO JOÃO", "JOÃO MACEDO");

        // Act
        var result = store.ToString();

        // Assert
        result.Should().Contain("BAR DO JOÃO");
        result.Should().Contain("JOÃO MACEDO");
    }
}
