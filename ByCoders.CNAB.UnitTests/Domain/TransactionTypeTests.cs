using ByCoders.CNAB.Domain.Transactions.Models;
using FluentAssertions;
using Xunit;

namespace ByCoders.CNAB.UnitTests.Domain;

public class TransactionTypeTests
{
    public TransactionTypeTests()
    {
        
    }

    [Fact]
    public void TransactionType_WithTransactionTypes_ShouldSetId()
    {
        // Arrange
        var type = TransactionTypes.Sales;

        // Act
        var transactionType = new TransactionType(type);

        // Assert
        transactionType.Id.Should().Be((int)type);
    }

    [Fact]
    public void TransactionType_WithDescriptionAndNature_ShouldSetAllProperties()
    {
        // Arrange
        var type = TransactionTypes.Debit;
        var description = "Débito";
        var nature = TransactionNature.CashIn;

        // Act
        var transactionType = new TransactionType(type, description, nature);

        // Assert
        transactionType.Id.Should().Be((int)type);
        transactionType.Description.Should().Be(description);
        transactionType.Nature.Should().Be(nature);
    }

    [Theory]
    [InlineData(TransactionTypes.Debit, 1)]
    [InlineData(TransactionTypes.BankSlip, 2)]
    [InlineData(TransactionTypes.Funding, 3)]
    [InlineData(TransactionTypes.Credit, 4)]
    [InlineData(TransactionTypes.LoanReceipt, 5)]
    [InlineData(TransactionTypes.Sales, 6)]
    [InlineData(TransactionTypes.TEDReceipt, 7)]
    [InlineData(TransactionTypes.DOCReceipt, 8)]
    [InlineData(TransactionTypes.Rent, 9)]
    public void TransactionType_ShouldMapEnumToCorrectId(TransactionTypes type, int expectedId)
    {
        // Arrange & Act
        var transactionType = new TransactionType(type);

        // Assert
        transactionType.Id.Should().Be(expectedId);
    }

    [Fact]
    public void TransactionType_WithSameId_ShouldBeEqual()
    {
        // Arrange
        var type1 = new TransactionType(TransactionTypes.Sales);
        var type2 = new TransactionType(TransactionTypes.Sales);

        // Act & Assert
        type1.Should().Be(type2);
        type1.GetHashCode().Should().Be(type2.GetHashCode());
    }

    [Fact]
    public void TransactionType_WithDifferentId_ShouldNotBeEqual()
    {
        // Arrange
        var type1 = new TransactionType(TransactionTypes.Sales);
        var type2 = new TransactionType(TransactionTypes.Debit);

        // Act & Assert
        type1.Should().NotBe(type2);
    }

    [Fact]
    public void TransactionNature_CashIn_ShouldBe1()
    {
        // Act & Assert
        TransactionNature.CashIn.Should().Be((TransactionNature)1);
    }

    [Fact]
    public void TransactionNature_CashOut_ShouldBe2()
    {
        // Act & Assert
        TransactionNature.CashOut.Should().Be((TransactionNature)2);
    }
}
