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
        result.Should().NotBeNull();
        result.TransactionType.Should().Be(TransactionTypes.Funding);
        result.Date.Should().Be(new DateOnly(2019, 03, 01));
        result.Amount.Should().Be(142);
        result.CPF.Should().Be("09620676017");
        result.CardNumber.Should().Be("4753****3153");
        result.Time.Should().Be(new TimeOnly(15, 34, 53));
        result.StoreOwner.Should().Be("JOÃO MACEDO");
        result.StoreName.Should().Be("BAR DO JOÃO");
    }

    [Fact]
    public void Parse_DebitTransaction_ShouldReturnDebitType()
    {
        // Arrange
        var line = "1201903010000015200096206760171234****7890233000JOÃO MACEDO   BAR DO JOÃO       ";

        // Act
        var result = _sut.Parse(line);

        // Assert
        result.TransactionType.Should().Be(TransactionTypes.Debit);
    }

    [Fact]
    public void Parse_SalesTransaction_ShouldReturnSalesType()
    {
        // Arrange
        var line = "6201903010000015200096206760171234****7890233000JOÃO MACEDO   BAR DO JOÃO       ";

        // Act
        var result = _sut.Parse(line);

        // Assert
        result.TransactionType.Should().Be(TransactionTypes.Sales);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void Parse_EmptyOrNullLine_ShouldThrowArgumentException(string line)
    {
        // Act
        var act = () => _sut.Parse(line);

        // Assert
        act.Should().Throw<ArgumentException>()
            .WithMessage("CNAB line cannot be empty*");
    }

    [Fact]
    public void Parse_LineTooShort_ShouldThrowArgumentException()
    {
        // Arrange
        var line = "123456789"; // Less than 81 characters

        // Act
        var act = () => _sut.Parse(line);

        // Assert
        act.Should().Throw<ArgumentException>()
            .WithMessage("CNAB line must be at least 81 characters*");
    }

    [Theory]
    [InlineData("0")]  // Below range
    [InlineData("10")] // Above range
    [InlineData("A")]  // Not a number
    public void Parse_InvalidTransactionType_ShouldThrowArgumentException(string transactionType)
    {
        // Arrange
        var line = $"{transactionType}201903010000014200096206760174753****3153153453JOÃO MACEDO   BAR DO JOÃO       ";

        // Act
        var act = () => _sut.Parse(line);

        // Assert
        act.Should().Throw<ArgumentException>()
            .WithMessage("Invalid transaction type*");
    }

    [Fact]
    public void Parse_InvalidDate_ShouldThrowArgumentException()
    {
        // Arrange
        var line = "3201913990000014200096206760174753****3153153453JOÃO MACEDO   BAR DO JOÃO       ";
        // Date: 20191399 (invalid)

        // Act
        var act = () => _sut.Parse(line);

        // Assert
        act.Should().Throw<ArgumentException>();
    }

    [Fact]
    public void Parse_InvalidAmount_ShouldThrowArgumentException()
    {
        // Arrange
        var line = "320190301ABCDEFGHIJ00096206760174753****3153153453JOÃO MACEDO   BAR DO JOÃO       ";
        // Amount contains letters

        // Act
        var act = () => _sut.Parse(line);

        // Assert
        act.Should().Throw<ArgumentException>()
            .WithMessage("Invalid amount format*");
    }

    [Fact]
    public void Parse_InvalidTime_ShouldThrowArgumentException()
    {
        // Arrange
        var line = "3201903010000014200096206760174753****999999JOÃO MACEDO   BAR DO JOÃO       ";
        // Time: 999999 (invalid hour)

        // Act
        var act = () => _sut.Parse(line);

        // Assert
        act.Should().Throw<ArgumentException>();
    }

    [Fact]
    public void Parse_AllTransactionTypes_ShouldParseCorrectly()
    {
        // Arrange & Act & Assert
        for (int i = 1; i <= 9; i++)
        {
            var line = $"{i}201903010000014200096206760174753****3153153453JOÃO MACEDO   BAR DO JOÃO       ";
            var result = _sut.Parse(line);
            result.TransactionType.Should().Be((TransactionTypes)i);
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
        result.StoreOwner.Should().Be("JOÃO MACEDO");
        result.StoreName.Should().Be("BAR DO JOÃO");
        result.CPF.Should().NotContain(" ");
        result.CardNumber.Should().NotContain(" ");
    }

    [Fact]
    public void Parse_ZeroAmount_ShouldParseCorrectly()
    {
        // Arrange
        var line = "3201903010000000000096206760174753****3153153453JOÃO MACEDO   BAR DO JOÃO       ";

        // Act
        var result = _sut.Parse(line);

        // Assert
        result.Amount.Should().Be(0);
    }

    [Fact]
    public void Parse_LargeAmount_ShouldParseCorrectly()
    {
        // Arrange
        var line = "3201903019999999999096206760174753****3153153453JOÃO MACEDO   BAR DO JOÃO       ";

        // Act
        var result = _sut.Parse(line);

        // Assert
        result.Amount.Should().Be(9999999999);
    }
}
