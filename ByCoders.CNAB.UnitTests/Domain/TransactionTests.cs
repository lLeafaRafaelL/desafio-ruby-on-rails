using ByCoders.CNAB.Domain.Transactions.Models;
using FluentAssertions;
using Xunit;

namespace ByCoders.CNAB.UnitTests.Domain;

public class TransactionTests
{
    private readonly Beneficiary _beneficiary = new("12345678901");
    private readonly Card _card = new("1234****5678");
    private readonly Store _store = new("Test Store", "John Doe");
    private readonly DateOnly _date = new(2019, 03, 01);
    private readonly TimeOnly _time = new(15, 30, 45);

    [Fact]
    public void Transaction_WhenCreated_ShouldGenerateId()
    {
        // Arrange & Act
        var transaction = new Sale(_date, _time, 1000, _beneficiary, _card, _store);

        // Assert
        transaction.Id.Should().NotBeEmpty();
    }

    [Fact]
    public void Transaction_WhenCreated_ShouldSetCreatedOn()
    {
        // Arrange
        var before = DateTimeOffset.UtcNow;

        // Act
        var transaction = new Sale(_date, _time, 1000, _beneficiary, _card, _store);

        // Assert
        var after = DateTimeOffset.UtcNow;
        transaction.CreatedOn.Should().BeOnOrAfter(before);
        transaction.CreatedOn.Should().BeOnOrBefore(after);
    }

    [Fact]
    public void Transaction_WhenCreated_ShouldSetAllProperties()
    {
        // Arrange
        var amount = 14200m;

        // Act
        var transaction = new Sale(_date, _time, amount, _beneficiary, _card, _store);

        // Assert
        transaction.TransactionDate.Should().Be(_date);
        transaction.TransactionTime.Should().Be(_time);
        transaction.AmountCNAB.Should().Be(amount);
        transaction.Beneficiary.Should().Be(_beneficiary);
        transaction.Card.Should().Be(_card);
        transaction.Store.Should().Be(_store);
    }

    [Fact]
    public void Transaction_BaseTransactionValue_ShouldDivideBy100()
    {
        // Arrange
        var amount = 14200m;

        // Act
        var transaction = new Sale(_date, _time, amount, _beneficiary, _card, _store);

        // Assert
        transaction.TransactionValue.Should().Be(142m);
    }

    [Fact]
    public void Transaction_WithZeroAmount_ShouldHaveZeroValue()
    {
        // Arrange & Act
        var transaction = new Sale(_date, _time, 0, _beneficiary, _card, _store);

        // Assert
        transaction.AmountCNAB.Should().Be(0);
        transaction.TransactionValue.Should().Be(0);
    }

    [Fact]
    public void Transaction_MultipleInstances_ShouldHaveUniqueIds()
    {
        // Arrange & Act
        var transaction1 = new Sale(_date, _time, 1000, _beneficiary, _card, _store);
        var transaction2 = new Sale(_date, _time, 1000, _beneficiary, _card, _store);

        // Assert
        transaction1.Id.Should().NotBe(transaction2.Id);
    }

    [Fact]
    public void Transaction_Beneficiary_ShouldStoreDocument()
    {
        // Arrange
        var cpf = "09620676017";
        var beneficiary = new Beneficiary(cpf);

        // Act
        var transaction = new Sale(_date, _time, 1000, beneficiary, _card, _store);

        // Assert
        transaction.Beneficiary.Document.Should().Be(cpf);
    }

    [Fact]
    public void Transaction_Card_ShouldStoreNumber()
    {
        // Arrange
        var cardNumber = "4753****3153";
        var card = new Card(cardNumber);

        // Act
        var transaction = new Sale(_date, _time, 1000, _beneficiary, card, _store);

        // Assert
        transaction.Card.Number.Should().Be(cardNumber);
    }

    [Fact]
    public void Transaction_Store_ShouldStoreNameAndOwner()
    {
        // Arrange
        var storeName = "BAR DO JOÃO";
        var storeOwner = "JOÃO MACEDO";
        var store = new Store(storeName, storeOwner);

        // Act
        var transaction = new Sale(_date, _time, 1000, _beneficiary, _card, store);

        // Assert
        transaction.Store.Name.Should().Be(storeName);
        transaction.Store.Owner.Should().Be(storeOwner);
    }

    [Theory]
    [InlineData(100, 1)]
    [InlineData(1000, 10)]
    [InlineData(10000, 100)]
    [InlineData(14200, 142)]
    public void Transaction_TransactionValue_ShouldCalculateCorrectly(decimal amountCNAB, decimal expectedValue)
    {
        // Arrange & Act
        var transaction = new Sale(_date, _time, amountCNAB, _beneficiary, _card, _store);

        // Assert
        transaction.TransactionValue.Should().Be(expectedValue);
    }
}
