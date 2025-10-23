using ByCoders.CNAB.AppService.Transactions.CNAB.Import;
using ByCoders.CNAB.Domain.Transactions.Models;
using FluentAssertions;
using Xunit;

namespace ByCoders.CNAB.UnitTests.AppService;

public class CNABLineParserTests
{
    private readonly CNABLineParser _sut;

    public CNABLineParserTests()
    {
        _sut = new CNABLineParser();
    }

    [Fact]
    public void Parse_ValidCNABLine_ShouldReturnCorrectData()
    {
        // Arrange
        var line = "3201903010000014200096206760174753****3153153453JOÃO MACEDO   BAR DO JOÃO       ";

        // Act
        var result = _sut.Parse(line);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeNull();
        result.Value.TransactionType.Should().Be(TransactionTypes.Funding);
        result.Value.Date.Should().Be(new DateOnly(2019, 03, 01));
        result.Value.Amount.Should().Be(142);
        result.Value.CPF.Should().Be("09620676017");
        result.Value.CardNumber.Should().Be("4753****3153");
        result.Value.Time.Should().Be(new TimeOnly(15, 34, 53));
        result.Value.StoreOwner.Should().Be("JOÃO MACEDO");
        result.Value.StoreName.Should().Be("BAR DO JOÃO");
    }

    [Fact]
    public void Parse_DebitTransaction_ShouldReturnDebitType()
    {
        // Arrange
        var line = "1201903010000015200096206760171234****7890233000JOÃO MACEDO   BAR DO JOÃO       ";

        // Act
        var result = _sut.Parse(line);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.TransactionType.Should().Be(TransactionTypes.Debit);
    }

    [Fact]
    public void Parse_SalesTransaction_ShouldReturnSalesType()
    {
        // Arrange
        var line = "6201903010000015200096206760171234****7890233000JOÃO MACEDO   BAR DO JOÃO       ";

        // Act
        var result = _sut.Parse(line);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.TransactionType.Should().Be(TransactionTypes.Sales);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void Parse_EmptyOrNullLine_ShouldReturnFailure(string line)
    {
        // Act
        var result = _sut.Parse(line);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Contain("CNAB line cannot be empty");
    }

    [Fact]
    public void Parse_LineTooShort_ShouldReturnFailure()
    {
        // Arrange
        var line = "123456789"; // Less than 80 characters

        // Act
        var result = _sut.Parse(line);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Contain("CNAB line must be at least 80 characters");
    }

    [Theory]
    [InlineData("0")]  // Below range
    [InlineData("A")]  // Not a number
    public void Parse_InvalidTransactionType_ShouldReturnFailure(string transactionType)
    {
        // Arrange - Using a valid 80-char line and replacing the transaction type
        var baseLine = "3201903010000014200096206760174753****3153153453JOÃO MACEDO   BAR DO JOÃO       ";
        var line = transactionType + baseLine.Substring(1);

        // Act
        var result = _sut.Parse(line);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Contain("Invalid transaction type");
    }

    [Fact]
    public void Parse_InvalidDate_ShouldReturnFailure()
    {
        // Arrange
        var line = "3201913990000014200096206760174753****3153153453JOÃO MACEDO   BAR DO JOÃO       ";
        // Date: 20191399 (invalid)

        // Act
        var result = _sut.Parse(line);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().NotBeEmpty();
    }

    [Fact]
    public void Parse_InvalidAmount_ShouldReturnFailure()
    {
        // Arrange
        var line = "320190301ABCDEFGHIJ00096206760174753****3153153453JOÃO MACEDO   BAR DO JOÃO       ";
        // Amount contains letters

        // Act
        var result = _sut.Parse(line);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Contain("Invalid amount format");
    }

    [Fact]
    public void Parse_InvalidTime_ShouldReturnFailure()
    {
        // Arrange
        var line = "3201903010000014200096206760174753****999999JOÃO MACEDO   BAR DO JOÃO       ";
        // Time: 999999 (invalid hour)

        // Act
        var result = _sut.Parse(line);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().NotBeEmpty();
    }

    [Fact]
    public void Parse_AllTransactionTypes_ShouldParseCorrectly()
    {
        // Arrange & Act & Assert
        for (int i = 1; i <= 9; i++)
        {
            var line = $"{i}201903010000014200096206760174753****3153153453JOÃO MACEDO   BAR DO JOÃO       ";
            var result = _sut.Parse(line);
            result.IsSuccess.Should().BeTrue();
            result.Value.TransactionType.Should().Be((TransactionTypes)i);
        }
    }

    [Fact]
    public void Parse_WithLeadingAndTrailingSpaces_ShouldTrimCorrectly()
    {
        // Arrange
        var line = "3201903010000014200096206760174753****3153153453  JOÃO MACEDO    BAR DO JOÃO     ";

        // Act
        var result = _sut.Parse(line);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.StoreOwner.Should().Be("JOÃO MACEDO");
        result.Value.StoreName.Should().Be("BAR DO JOÃO");
        result.Value.CPF.Should().NotContain(" ");
        result.Value.CardNumber.Should().NotContain(" ");
    }

    [Fact]
    public void Parse_ZeroAmount_ShouldParseCorrectly()
    {
        // Arrange
        var line = "3201903010000000000096206760174753****3153153453JOÃO MACEDO   BAR DO JOÃO       ";

        // Act
        var result = _sut.Parse(line);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Amount.Should().Be(0);
    }

    [Fact]
    public void Parse_LargeAmount_ShouldParseCorrectly()
    {
        // Arrange
        var line = "3201903019999999999096206760174753****3153153453JOÃO MACEDO   BAR DO JOÃO       ";

        // Act
        var result = _sut.Parse(line);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Amount.Should().Be(99999999.99m); // Amount is divided by 100
    }
}
