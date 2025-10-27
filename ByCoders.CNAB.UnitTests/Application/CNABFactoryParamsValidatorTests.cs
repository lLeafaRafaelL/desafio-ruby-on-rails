using ByCoders.CNAB.Application.Transactions;
using ByCoders.CNAB.Domain.Transactions.Models;
using FluentAssertions;
using FluentValidation.TestHelper;
using Xunit;

namespace ByCoders.CNAB.UnitTests.Application;

public class CNABFactoryParamsValidatorTests
{
    private readonly CNABFactoryParamsValidator _validator;

    public CNABFactoryParamsValidatorTests()
    {
        _validator = new CNABFactoryParamsValidator();
    }

    [Fact]
    public void Validate_WithValidData_ShouldNotHaveValidationError()
    {
        // Arrange
        var data = CreateValidDto();

        // Act
        var result = _validator.TestValidate(data);

        // Assert
        result.ShouldNotHaveAnyValidationErrors();
    }

    // Removed: FluentValidation doesn't accept null models - validation is handled in TransactionFactory.Create()

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void Validate_WithEmptyCPF_ShouldHaveValidationError(string cpf)
    {
        // Arrange
        var data = CreateValidDto() with { CPF = cpf };

        // Act
        var result = _validator.TestValidate(data);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.CPF)
            .WithErrorMessage("CPF cannot be empty");
    }

    [Fact]
    public void Validate_WithCPFDifferentFrom11Characters_ShouldHaveValidationError()
    {
        // Arrange
        var data = CreateValidDto() with { CPF = "123456789" }; // 9 characters

        // Act
        var result = _validator.TestValidate(data);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.CPF)
            .WithErrorMessage("CPF must have 11 characters");
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void Validate_WithEmptyCardNumber_ShouldHaveValidationError(string cardNumber)
    {
        // Arrange
        var data = CreateValidDto() with { CardNumber = cardNumber };

        // Act
        var result = _validator.TestValidate(data);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.CardNumber)
            .WithErrorMessage("Card number cannot be empty");
    }

    [Fact]
    public void Validate_WithCardNumberLessThan4Characters_ShouldHaveValidationError()
    {
        // Arrange
        var data = CreateValidDto() with { CardNumber = "123" };

        // Act
        var result = _validator.TestValidate(data);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.CardNumber)
            .WithErrorMessage("Card number must have at least 4 characters");
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void Validate_WithEmptyStoreName_ShouldHaveValidationError(string storeName)
    {
        // Arrange
        var data = CreateValidDto() with { StoreName = storeName };

        // Act
        var result = _validator.TestValidate(data);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.StoreName)
            .WithErrorMessage("Store name cannot be empty");
    }

    [Fact]
    public void Validate_WithStoreNameExceedingMaxLength_ShouldHaveValidationError()
    {
        // Arrange
        var longName = new string('A', 101); // 101 characters
        var data = CreateValidDto() with { StoreName = longName };

        // Act
        var result = _validator.TestValidate(data);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.StoreName)
            .WithErrorMessage("Store name must not exceed 100 characters");
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void Validate_WithEmptyStoreOwner_ShouldHaveValidationError(string storeOwner)
    {
        // Arrange
        var data = CreateValidDto() with { StoreOwner = storeOwner };

        // Act
        var result = _validator.TestValidate(data);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.StoreOwner)
            .WithErrorMessage("Store owner cannot be empty");
    }

    [Fact]
    public void Validate_WithStoreOwnerExceedingMaxLength_ShouldHaveValidationError()
    {
        // Arrange
        var longOwner = new string('B', 101); // 101 characters
        var data = CreateValidDto() with { StoreOwner = longOwner };

        // Act
        var result = _validator.TestValidate(data);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.StoreOwner)
            .WithErrorMessage("Store owner must not exceed 100 characters");
    }

    [Fact]
    public void Validate_WithNegativeAmount_ShouldHaveValidationError()
    {
        // Arrange
        var data = CreateValidDto() with { Amount = -100 };

        // Act
        var result = _validator.TestValidate(data);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Amount)
            .WithErrorMessage("Amount cannot be negative");
    }

    [Theory]
    [InlineData(0)]
    [InlineData(100)]
    [InlineData(999999)]
    public void Validate_WithValidAmount_ShouldNotHaveValidationError(decimal amount)
    {
        // Arrange
        var data = CreateValidDto() with { Amount = amount };

        // Act
        var result = _validator.TestValidate(data);

        // Assert
        result.ShouldNotHaveValidationErrorFor(x => x.Amount);
    }

    [Fact]
    public void Validate_WithMultipleErrors_ShouldReturnAllErrors()
    {
        // Arrange
        var data = new CNABFactoryParams(
            TransactionTypes.Sales,
            new DateOnly(2019, 03, 01),
            -1000, // Invalid amount
            "",    // Invalid CPF
            "12",  // Invalid card (too short)
            new TimeOnly(10, 00, 00),
            "",    // Invalid owner
            ""     // Invalid store name
        );

        // Act
        var result = _validator.Validate(data);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().HaveCount(6); // 5 validation errors
        result.Errors.Should().Contain(e => e.PropertyName == "CPF");
        result.Errors.Should().Contain(e => e.PropertyName == "CardNumber");
        result.Errors.Should().Contain(e => e.PropertyName == "StoreName");
        result.Errors.Should().Contain(e => e.PropertyName == "StoreOwner");
        result.Errors.Should().Contain(e => e.PropertyName == "Amount");
    }

    [Fact]
    public void Validate_WithValidTransactionType_ShouldNotHaveValidationError()
    {
        // Arrange - All valid transaction types
        var types = new[]
        {
            TransactionTypes.Debit,
            TransactionTypes.BankSlip,
            TransactionTypes.Funding,
            TransactionTypes.Credit,
            TransactionTypes.LoanReceipt,
            TransactionTypes.Sales,
            TransactionTypes.TEDReceipt,
            TransactionTypes.DOCReceipt,
            TransactionTypes.Rent
        };

        foreach (var type in types)
        {
            var data = CreateValidDto() with { TransactionType = type };

            // Act
            var result = _validator.TestValidate(data);

            // Assert
            result.ShouldNotHaveValidationErrorFor(x => x.TransactionType);
        }
    }

    [Fact]
    public void Validate_WithRealWorldExample_ShouldBeValid()
    {
        // Arrange - Dados reais do arquivo CNAB
        var data = new CNABFactoryParams(
            TransactionTypes.Funding,
            new DateOnly(2019, 03, 01),
            14200m,
            "09620676017",
            "4753****3153",
            new TimeOnly(15, 34, 53),
            "JOÃO MACEDO",
            "BAR DO JOÃO"
        );

        // Act
        var result = _validator.TestValidate(data);

        // Assert
        result.ShouldNotHaveAnyValidationErrors();
    }

    // Helper method
    private CNABFactoryParams CreateValidDto()
    {
        return new CNABFactoryParams(
            TransactionTypes.Sales,
            new DateOnly(2019, 03, 01),
            10000m,
            "12345678901", // 11 characters
            "1234****5678",
            new TimeOnly(10, 00, 00),
            "John Doe",
            "Test Store"
        );
    }
}
