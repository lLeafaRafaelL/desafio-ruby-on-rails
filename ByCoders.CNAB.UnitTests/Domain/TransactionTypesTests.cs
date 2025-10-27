using ByCoders.CNAB.Domain.Transactions.Models;
using ByCoders.CNAB.UnitTests.Builders.Domain;
using FluentAssertions;
using Xunit;

namespace ByCoders.CNAB.UnitTests.Domain;

public class TransactionTypesTests
{

    #region Cash In Transactions (Positive Values)

    [Fact]
    public void Debit_ShouldHavePositiveValue()
    {
        // Arrange & Act
        var transaction = DebitBuilder.New
            .WithAmountCNAB(10000m)
            .Build();

        // Assert
        transaction.AmountCNAB.Should().Be(10000m);
        transaction.TransactionValue.Should().Be(100m); // Positive: Cash In
    }

    [Fact]
    public void Credit_ShouldHavePositiveValue()
    {
        // Arrange & Act
        var transaction = CreditBuilder.New
            .WithAmountCNAB(10000m)
            .Build();

        // Assert
        transaction.AmountCNAB.Should().Be(10000m);
        transaction.TransactionValue.Should().Be(100m); // Positive: Cash In
    }

    [Fact]
    public void Sale_ShouldHavePositiveValue()
    {
        // Arrange & Act
        var transaction = SaleBuilder.New
            .WithAmountCNAB(10000m)
            .Build();

        // Assert
        transaction.AmountCNAB.Should().Be(10000m);
        transaction.TransactionValue.Should().Be(100m); // Positive: Cash In
    }

    [Fact]
    public void LoanReceipt_ShouldHavePositiveValue()
    {
        // Arrange & Act
        var transaction = LoanReceiptBuilder.New
            .WithAmountCNAB(10000m)
            .Build();

        // Assert
        transaction.AmountCNAB.Should().Be(10000m);
        transaction.TransactionValue.Should().Be(100m); // Positive: Cash In
    }

    [Fact]
    public void TEDReceipt_ShouldHavePositiveValue()
    {
        // Arrange & Act
        var transaction = TEDReceiptBuilder.New
            .WithAmountCNAB(10000m)
            .Build();

        // Assert
        transaction.AmountCNAB.Should().Be(10000m);
        transaction.TransactionValue.Should().Be(100m); // Positive: Cash In
    }

    [Fact]
    public void DOCReceipt_ShouldHavePositiveValue()
    {
        // Arrange & Act
        var transaction = DOCReceiptBuilder.New
            .WithAmountCNAB(10000m)
            .Build();

        // Assert
        transaction.AmountCNAB.Should().Be(10000m);
        transaction.TransactionValue.Should().Be(100m); // Positive: Cash In
    }

    #endregion

    #region Cash Out Transactions (Negative Values)

    [Fact]
    public void BankSlip_ShouldHaveNegativeValue()
    {
        // Arrange & Act
        var transaction = BankSlipBuilder.New
            .WithAmountCNAB(10000m)
            .Build();

        // Assert
        transaction.AmountCNAB.Should().Be(10000m);
        transaction.TransactionValue.Should().Be(-100m); // Negative: Cash Out
    }

    [Fact]
    public void Funding_ShouldHaveNegativeValue()
    {
        // Arrange & Act
        var transaction = FundingBuilder.New
            .WithAmountCNAB(10000m)
            .Build();

        // Assert
        transaction.AmountCNAB.Should().Be(10000m);
        transaction.TransactionValue.Should().Be(-100m); // Negative: Cash Out
    }

    [Fact]
    public void Rent_ShouldHaveNegativeValue()
    {
        // Arrange & Act
        var transaction = RentBuilder.New
            .WithAmountCNAB(10000m)
            .Build();

        // Assert
        transaction.AmountCNAB.Should().Be(10000m);
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
            TransactionTypes.Debit => DebitBuilder.New.WithAmountCNAB(10000m).Build(),
            TransactionTypes.BankSlip => BankSlipBuilder.New.WithAmountCNAB(10000m).Build(),
            TransactionTypes.Funding => FundingBuilder.New.WithAmountCNAB(10000m).Build(),
            TransactionTypes.Credit => CreditBuilder.New.WithAmountCNAB(10000m).Build(),
            TransactionTypes.LoanReceipt => LoanReceiptBuilder.New.WithAmountCNAB(10000m).Build(),
            TransactionTypes.Sales => SaleBuilder.New.WithAmountCNAB(10000m).Build(),
            TransactionTypes.TEDReceipt => TEDReceiptBuilder.New.WithAmountCNAB(10000m).Build(),
            TransactionTypes.DOCReceipt => DOCReceiptBuilder.New.WithAmountCNAB(10000m).Build(),
            TransactionTypes.Rent => RentBuilder.New.WithAmountCNAB(10000m).Build(),
            _ => throw new ArgumentException("Invalid type")
        };

        // Assert
        transaction.Should().BeOfType(expectedType);
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
        // Arrange & Act
        var transaction = SaleBuilder.New
            .WithAmountCNAB(999999999m)
            .Build();

        // Assert
        transaction.TransactionValue.Should().Be(9999999.99m);
    }

    [Fact]
    public void Transaction_WithDecimalAmount_ShouldMaintainPrecision()
    {
        // Arrange & Act
        var transaction = SaleBuilder.New
            .WithAmountCNAB(12345m) // BRL 123.45
            .Build();

        // Assert
        transaction.TransactionValue.Should().Be(123.45m);
    }

    [Fact]
    public void NegativeTransaction_WithDecimalAmount_ShouldMaintainNegativePrecision()
    {
        // Arrange & Act
        var transaction = BankSlipBuilder.New
            .WithAmountCNAB(12345m) // BRL 123.45
            .Build();

        // Assert
        transaction.TransactionValue.Should().Be(-123.45m);
    }

    #endregion

    #region Business Rules

    [Theory]
    [InlineData(TransactionTypes.Debit, 1)]
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
            TransactionTypes.Debit => DebitBuilder.New.WithAmountCNAB(10000m).Build(),
            TransactionTypes.BankSlip => BankSlipBuilder.New.WithAmountCNAB(10000m).Build(),
            TransactionTypes.Funding => FundingBuilder.New.WithAmountCNAB(10000m).Build(),
            TransactionTypes.Rent => RentBuilder.New.WithAmountCNAB(10000m).Build(),
            TransactionTypes.Credit => CreditBuilder.New.WithAmountCNAB(10000m).Build(),
            TransactionTypes.LoanReceipt => LoanReceiptBuilder.New.WithAmountCNAB(10000m).Build(),
            TransactionTypes.Sales => SaleBuilder.New.WithAmountCNAB(10000m).Build(),
            TransactionTypes.TEDReceipt => TEDReceiptBuilder.New.WithAmountCNAB(10000m).Build(),
            TransactionTypes.DOCReceipt => DOCReceiptBuilder.New.WithAmountCNAB(10000m).Build(),
            _ => throw new ArgumentException("Invalid type")
        };

        // Assert
        Math.Sign(transaction.TransactionValue).Should().Be(expectedSign);
    }

    #endregion
}
