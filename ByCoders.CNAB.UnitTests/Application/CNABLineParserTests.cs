using ByCoders.CNAB.Application.Files.CNAB.Parsers;
using ByCoders.CNAB.Domain.Transactions.Models;
using FluentAssertions;
using Xunit;

namespace ByCoders.CNAB.UnitTests.Application;

public class CNABLineParserTests
{
    private readonly CNABLineParser _parser;

    public CNABLineParserTests()
    {
        _parser = new CNABLineParser();
    }

    [Fact]
    public void Parse_WhenValidCNABLineProvided_ShouldReturnParsedDataWithAllFieldsCorrect()
    {
        // Arrange
        const string validCnabLine = "3201903010000014200096206760174753****3153153453JOÃO MACEDO   BAR DO JOÃO       ";

        // Act
        var parseResult = _parser.Parse(validCnabLine);

        // Assert
        parseResult.Succeeded.Should().BeTrue();
        parseResult.Value.Should().NotBeNull();
        parseResult.Value.TransactionType.Should().Be(TransactionTypes.Funding);
        parseResult.Value.Date.Should().Be(new DateOnly(2019, 03, 01));
        parseResult.Value.Amount.Should().Be(142m);
        parseResult.Value.CPF.Should().Be("09620676017");
        parseResult.Value.CardNumber.Should().Be("4753****3153");
        parseResult.Value.Time.Should().Be(new TimeOnly(15, 34, 53));
        parseResult.Value.StoreOwner.Should().Be("JOÃO MACEDO");
        parseResult.Value.StoreName.Should().Be("BAR DO JOÃO");
    }

    [Fact]
    public void Parse_WhenLineStartsWithDebitCode_ShouldReturnDebitTransactionType()
    {
        // Arrange
        const string debitTransactionLine = "1201903010000015200096206760171234****7890233000JOÃO MACEDO   BAR DO JOÃO       ";

        // Act
        var parseResult = _parser.Parse(debitTransactionLine);

        // Assert
        parseResult.Succeeded.Should().BeTrue();
        parseResult.Value?.TransactionType.Should().Be(TransactionTypes.Debit);
    }

    [Fact]
    public void Parse_WhenLineStartsWithSalesCode_ShouldReturnSalesTransactionType()
    {
        // Arrange
        const string salesTransactionLine = "6201903010000015200096206760171234****7890233000JOÃO MACEDO   BAR DO JOÃO       ";

        // Act
        var parseResult = _parser.Parse(salesTransactionLine);

        // Assert
        parseResult.Succeeded.Should().BeTrue();
        parseResult.Value?.TransactionType.Should().Be(TransactionTypes.Sales);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void Parse_WhenLineIsEmptyOrNull_ShouldReturnFailureWithAppropriateMessage(string? invalidLine)
    {
        // Act
        var parseResult = _parser.Parse(invalidLine!);

        // Assert
        parseResult.Succeeded.Should().BeFalse();
        parseResult.FailureDetails.Should().Contain(x => x.Description != null && x.Description.Contains("CNAB line cannot be empty"));
    }

    [Fact]
    public void Parse_WhenLineLengthLessThan80Characters_ShouldReturnFailureWithLengthError()
    {
        // Arrange
        const string shortLine = "123456789"; // Only 9 characters, minimum required is 80

        // Act
        var parseResult = _parser.Parse(shortLine);

        // Assert
        parseResult.Succeeded.Should().BeFalse();
        parseResult.FailureDetails.Should().Contain(x => x.Description != null && x.Description.Contains("CNAB line must be at least 80 characters"));
    }

    [Theory]
    [InlineData("0")]  // Below valid range (1-9)
    [InlineData("A")]  // Non-numeric character
    public void Parse_WhenTransactionTypeIsInvalid_ShouldReturnFailureWithInvalidTypeError(string invalidTransactionType)
    {
        // Arrange
        const string validLineTemplate = "3201903010000014200096206760174753****3153153453JOÃO MACEDO   BAR DO JOÃO       ";
        var lineWithInvalidType = invalidTransactionType + validLineTemplate.Substring(1);

        // Act
        var parseResult = _parser.Parse(lineWithInvalidType);

        // Assert
        parseResult.Succeeded.Should().BeFalse();
        parseResult.FailureDetails.Should().Contain(x => x.Description != null && x.Description.Contains("Invalid transaction type"));
    }

    [Fact]
    public void Parse_WhenDateIsInvalid_ShouldReturnFailureWithDateError()
    {
        // Arrange
        const string lineWithInvalidDate = "3201913990000014200096206760174753****3153153453JOÃO MACEDO   BAR DO JOÃO       ";
        // Date: 20191399 (month 13, day 99 - both invalid)

        // Act
        var parseResult = _parser.Parse(lineWithInvalidDate);

        // Assert
        parseResult.Succeeded.Should().BeFalse();
        parseResult.FailureDetails.Should().NotBeEmpty();
    }

    [Fact]
    public void Parse_WhenAmountContainsNonNumericCharacters_ShouldReturnFailureWithAmountError()
    {
        // Arrange
        const string lineWithInvalidAmount = "320190301ABCDEFGHIJ00096206760174753****3153153453JOÃO MACEDO   BAR DO JOÃO       ";
        // Amount field contains letters: ABCDEFGHIJ

        // Act
        var parseResult = _parser.Parse(lineWithInvalidAmount);

        // Assert
        parseResult.Succeeded.Should().BeFalse();
        parseResult.FailureDetails.Should().Contain(x => x.Description != null && x.Description.Contains("Invalid amount format"));
    }

    [Fact]
    public void Parse_WhenTimeIsInvalid_ShouldReturnFailureWithTimeError()
    {
        // Arrange
        const string lineWithInvalidTime = "3201903010000014200096206760174753****999999JOÃO MACEDO   BAR DO JOÃO       ";
        // Time: 999999 (hour 99 is invalid - max is 23)

        // Act
        var parseResult = _parser.Parse(lineWithInvalidTime);

        // Assert
        parseResult.Succeeded.Should().BeFalse();
        parseResult.FailureDetails.Should().NotBeEmpty();
    }

    [Theory]
    [InlineData(1, TransactionTypes.Debit)]
    [InlineData(2, TransactionTypes.BankSlip)]
    [InlineData(3, TransactionTypes.Funding)]
    [InlineData(4, TransactionTypes.Credit)]
    [InlineData(5, TransactionTypes.LoanReceipt)]
    [InlineData(6, TransactionTypes.Sales)]
    [InlineData(7, TransactionTypes.TEDReceipt)]
    [InlineData(8, TransactionTypes.DOCReceipt)]
    [InlineData(9, TransactionTypes.Rent)]
    public void Parse_WhenTransactionTypeIsValid_ShouldMapToCorrectTransactionType(int typeCode, TransactionTypes expectedType)
    {
        // Arrange
        var lineWithSpecificType = $"{typeCode}201903010000014200096206760174753****3153153453JOÃO MACEDO   BAR DO JOÃO       ";

        // Act
        var parseResult = _parser.Parse(lineWithSpecificType);

        // Assert
        parseResult.Succeeded.Should().BeTrue();
        parseResult.Value?.TransactionType.Should().Be(expectedType);
    }

    [Fact]
    public void Parse_WhenFieldsHaveExtraSpaces_ShouldTrimAllFieldsCorrectly()
    {
        // Arrange
        const string lineWithExtraSpaces = "3201903010000014200096206760174753****3153153453  JOÃO MACEDO    BAR DO JOÃO     ";

        // Act
        var parseResult = _parser.Parse(lineWithExtraSpaces);

        // Assert
        parseResult.Succeeded.Should().BeTrue();
        parseResult.Value?.StoreOwner.Should().Be("JOÃO MACEDO");
        parseResult.Value?.StoreName.Should().Be("BAR DO JOÃO");
        parseResult.Value?.CPF.Should().NotContain(" ");
        parseResult.Value?.CardNumber.Should().NotContain(" ");
    }

    [Fact]
    public void Parse_WhenAmountIsZero_ShouldReturnZeroAmount()
    {
        // Arrange
        const string lineWithZeroAmount = "3201903010000000000096206760174753****3153153453JOÃO MACEDO   BAR DO JOÃO       ";

        // Act
        var parseResult = _parser.Parse(lineWithZeroAmount);

        // Assert
        parseResult.Succeeded.Should().BeTrue();
        parseResult.Value?.Amount.Should().Be(0m);
    }

    [Fact]
    public void Parse_WhenAmountIsMaximumAllowed_ShouldConvertCorrectlyFromCentsToReais()
    {
        // Arrange
        const string lineWithMaxAmount = "3201903019999999999096206760174753****3153153453JOÃO MACEDO   BAR DO JOÃO       ";
        // Amount in cents: 9999999999

        // Act
        var parseResult = _parser.Parse(lineWithMaxAmount);

        // Assert
        parseResult.Succeeded.Should().BeTrue();
        parseResult.Value?.Amount.Should().Be(99999999.99m); // Converted from cents to reais (divided by 100)
    }
}
