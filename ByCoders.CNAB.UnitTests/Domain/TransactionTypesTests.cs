using ByCoders.CNAB.Domain.Transactions.Models;
using FluentAssertions;
using Xunit;

namespace ByCoders.CNAB.UnitTests.Domain;

public class TransactionTypesTests
{
    private readonly Beneficiary _beneficiary = new("12345678901");
    private readonly Card _card = new("1234****5678");
    private readonly Store _store = new("Test Store", "John Doe");
    private readonly DateOnly _date = new(2019, 03, 01);
    private readonly TimeOnly _time = new(15, 30, 45);
    private const decimal Amount = 10000m; // R$ 100.00

    #region Cash In Transactions (Positive Values)

    [Fact]
    public void Debit_ShouldHaveNegativeValue()
    {
        // Arrange & Act
        var transaction = new Debit(_date, _time, Amount, _beneficiary, _card, _store);

        // Assert
        transaction.TransactionType.Id.Should().Be((int)TransactionTypes.Debit);
        transaction.AmountCNAB.Should().Be(Amount);
        transaction.TransactionValue.Should().Be(-100m); // Negative: Cash Out
    }

    [Fact]
    public void Credit_ShouldHavePositiveValue()
    {
        // Arrange & Act
        var transaction = new Credit(_date, _time, Amount, _beneficiary, _card, _store);

        // Assert
        transaction.TransactionType.Id.Should().Be((int)TransactionTypes.Credit);
        transaction.AmountCNAB.Should().Be(Amount);
        transaction.TransactionValue.Should().Be(100m); // Positive: Cash In
    }

    [Fact]
    public void Sale_ShouldHavePositiveValue()
    {
        // Arrange & Act
        var transaction = new Sale(_date, _time, Amount, _beneficiary, _card, _store);

        // Assert
        transaction.TransactionType.Id.Should().Be((int)TransactionTypes.Sales);
        transaction.AmountCNAB.Should().Be(Amount);
        transaction.TransactionValue.Should().Be(100m); // Positive: Cash In
    }

    [Fact]
    public void LoanReceipt_ShouldHavePositiveValue()
    {
        // Arrange & Act
        var transaction = new LoanReceipt(_date, _time, Amount, _beneficiary, _card, _store);

        // Assert
        transaction.TransactionType.Id.Should().Be((int)TransactionTypes.LoanReceipt);
        transaction.AmountCNAB.Should().Be(Amount);
        transaction.TransactionValue.Should().Be(100m); // Positive: Cash In
    }

    [Fact]
    public void TEDReceipt_ShouldHavePositiveValue()
    {
        // Arrange & Act
        var transaction = new TEDReceipt(_date, _time, Amount, _beneficiary, _card, _store);

        // Assert
        transaction.TransactionType.Id.Should().Be((int)TransactionTypes.TEDReceipt);
        transaction.AmountCNAB.Should().Be(Amount);
        transaction.TransactionValue.Should().Be(100m); // Positive: Cash In
    }

    [Fact]
    public void DOCReceipt_ShouldHavePositiveValue()
    {
        // Arrange & Act
        var transaction = new DOCReceipt(_date, _time, Amount, _beneficiary, _card, _store);

        // Assert
        transaction.TransactionType.Id.Should().Be((int)TransactionTypes.DOCReceipt);
        transaction.AmountCNAB.Should().Be(Amount);
        transaction.TransactionValue.Should().Be(100m); // Positive: Cash In
    }

    #endregion

    #region Cash Out Transactions (Negative Values)

    [Fact]
    public void BankSlip_ShouldHaveNegativeValue()
    {
        // Arrange & Act
        var transaction = new BankSlip(_date, _time, Amount, _beneficiary, _card, _store);

        // Assert
        transaction.TransactionType.Id.Should().Be((int)TransactionTypes.BankSlip);
        transaction.AmountCNAB.Should().Be(Amount);
        transaction.TransactionValue.Should().Be(-100m); // Negative: Cash Out
    }

    [Fact]
    public void Funding_ShouldHaveNegativeValue()
    {
        // Arrange & Act
        var transaction = new Funding(_date, _time, Amount, _beneficiary, _card, _store);

        // Assert
        transaction.TransactionType.Id.Should().Be((int)TransactionTypes.Funding);
        transaction.AmountCNAB.Should().Be(Amount);
        transaction.TransactionValue.Should().Be(-100m); // Negative: Cash Out
    }

    [Fact]
    public void Rent_ShouldHaveNegativeValue()
    {
        // Arrange & Act
        var transaction = new Rent(_date, _time, Amount, _beneficiary, _card, _store);

        // Assert
        transaction.TransactionType.Id.Should().Be((int)TransactionTypes.Rent);
        transaction.AmountCNAB.Should().Be(Amount);
        transaction.TransactionValue.Should().Be(-100m); // Negative: Cash Out
    }

    #endregion

    #region Transaction Type Mapping

    [Theory]
    [InlineData(TransactionTypes.Debit, typeof(Debit))]
    [InlineData(TransactionTypes.BankSlip, typeof(BankSlip))]
    [InlineData(TransactionTypes.Funding, typeof(Funding))]
    [InlineData(TransactionTypes.Credit, typeof(Credit))]
    [InlineData(TransactionTypes.LoanReceipt, typeof(LoanReceipt))]
    [InlineData(TransactionTypes.Sales, typeof(Sale))]
    [InlineData(TransactionTypes.TEDReceipt, typeof(TEDReceipt))]
    [InlineData(TransactionTypes.DOCReceipt, typeof(DOCReceipt))]
    [InlineData(TransactionTypes.Rent, typeof(Rent))]
    public void AllTransactionTypes_ShouldMapToCorrectClass(TransactionTypes type, Type expectedType)
    {
        // Arrange & Act
        Transaction transaction = type switch
        {
            TransactionTypes.Debit => new Debit(_date, _time, Amount, _beneficiary, _card, _store),
            TransactionTypes.BankSlip => new BankSlip(_date, _time, Amount, _beneficiary, _card, _store),
            TransactionTypes.Funding => new Funding(_date, _time, Amount, _beneficiary, _card, _store),
            TransactionTypes.Credit => new Credit(_date, _time, Amount, _beneficiary, _card, _store),
            TransactionTypes.LoanReceipt => new LoanReceipt(_date, _time, Amount, _beneficiary, _card, _store),
            TransactionTypes.Sales => new Sale(_date, _time, Amount, _beneficiary, _card, _store),
            TransactionTypes.TEDReceipt => new TEDReceipt(_date, _time, Amount, _beneficiary, _card, _store),
            TransactionTypes.DOCReceipt => new DOCReceipt(_date, _time, Amount, _beneficiary, _card, _store),
            TransactionTypes.Rent => new Rent(_date, _time, Amount, _beneficiary, _card, _store),
            _ => throw new ArgumentException("Invalid type")
        };

        // Assert
        transaction.Should().BeOfType(expectedType);
        transaction.TransactionType.Id.Should().Be((int)type);
    }

    #endregion

    #region Value Object Tests

    [Fact]
    public void Beneficiary_WithSameDocument_ShouldBeEqual()
    {
        // Arrange
        var beneficiary1 = new Beneficiary("12345678901");
        var beneficiary2 = new Beneficiary("12345678901");

        // Act & Assert
        beneficiary1.Should().Be(beneficiary2);
        beneficiary1.GetHashCode().Should().Be(beneficiary2.GetHashCode());
    }

    [Fact]
    public void Card_WithSameNumber_ShouldBeEqual()
    {
        // Arrange
        var card1 = new Card("1234****5678");
        var card2 = new Card("1234****5678");

        // Act & Assert
        card1.Should().Be(card2);
        card1.GetHashCode().Should().Be(card2.GetHashCode());
    }

    [Fact]
    public void Store_WithSameNameAndOwner_ShouldBeEqual()
    {
        // Arrange
        var store1 = new Store("Test Store", "John Doe");
        var store2 = new Store("Test Store", "John Doe");

        // Act & Assert
        store1.Should().Be(store2);
        store1.GetHashCode().Should().Be(store2.GetHashCode());
    }

    [Fact]
    public void Store_WithDifferentOwner_ShouldNotBeEqual()
    {
        // Arrange
        var store1 = new Store("Test Store", "John Doe");
        var store2 = new Store("Test Store", "Jane Doe");

        // Act & Assert
        store1.Should().NotBe(store2);
    }

    #endregion

    #region Edge Cases

    [Fact]
    public void Transaction_WithLargeAmount_ShouldCalculateCorrectly()
    {
        // Arrange
        var largeAmount = 999999999m;

        // Act
        var transaction = new Sale(_date, _time, largeAmount, _beneficiary, _card, _store);

        // Assert
        transaction.TransactionValue.Should().Be(9999999.99m);
    }

    [Fact]
    public void Transaction_WithDecimalAmount_ShouldMaintainPrecision()
    {
        // Arrange
        var amount = 12345m; // R$ 123.45

        // Act
        var transaction = new Sale(_date, _time, amount, _beneficiary, _card, _store);

        // Assert
        transaction.TransactionValue.Should().Be(123.45m);
    }

    [Fact]
    public void NegativeTransaction_WithDecimalAmount_ShouldMaintainNegativePrecision()
    {
        // Arrange
        var amount = 12345m; // R$ 123.45

        // Act
        var transaction = new Debit(_date, _time, amount, _beneficiary, _card, _store);

        // Assert
        transaction.TransactionValue.Should().Be(-123.45m);
    }

    #endregion

    #region Business Rules

    [Theory]
    [InlineData(TransactionTypes.Debit, -1)]
    [InlineData(TransactionTypes.BankSlip, -1)]
    [InlineData(TransactionTypes.Funding, -1)]
    [InlineData(TransactionTypes.Rent, -1)]
    [InlineData(TransactionTypes.Credit, 1)]
    [InlineData(TransactionTypes.LoanReceipt, 1)]
    [InlineData(TransactionTypes.Sales, 1)]
    [InlineData(TransactionTypes.TEDReceipt, 1)]
    [InlineData(TransactionTypes.DOCReceipt, 1)]
    public void TransactionValue_ShouldHaveCorrectSign(TransactionTypes type, int expectedSign)
    {
        // Arrange & Act
        Transaction transaction = type switch
        {
            TransactionTypes.Debit => new Debit(_date, _time, Amount, _beneficiary, _card, _store),
            TransactionTypes.BankSlip => new BankSlip(_date, _time, Amount, _beneficiary, _card, _store),
            TransactionTypes.Funding => new Funding(_date, _time, Amount, _beneficiary, _card, _store),
            TransactionTypes.Rent => new Rent(_date, _time, Amount, _beneficiary, _card, _store),
            TransactionTypes.Credit => new Credit(_date, _time, Amount, _beneficiary, _card, _store),
            TransactionTypes.LoanReceipt => new LoanReceipt(_date, _time, Amount, _beneficiary, _card, _store),
            TransactionTypes.Sales => new Sale(_date, _time, Amount, _beneficiary, _card, _store),
            TransactionTypes.TEDReceipt => new TEDReceipt(_date, _time, Amount, _beneficiary, _card, _store),
            TransactionTypes.DOCReceipt => new DOCReceipt(_date, _time, Amount, _beneficiary, _card, _store),
            _ => throw new ArgumentException("Invalid type")
        };

        // Assert
        Math.Sign(transaction.TransactionValue).Should().Be(expectedSign);
    }

    #endregion
}
