using ByCoders.CNAB.Application.Transactions;
using ByCoders.CNAB.Core.Results;
using ByCoders.CNAB.Core.Validators;
using ByCoders.CNAB.Domain.Transactions.Models;
using ByCoders.CNAB.UnitTests.Builders.Application;
using FluentAssertions;
using NSubstitute;
using Xunit;

namespace ByCoders.CNAB.UnitTests.Application;

// Mock helper for validation results
internal static class ValidationResult
{
    public static DtoValidationResult Success() => 
        new DtoValidationResult(true);
    
    public static DtoValidationResult Failed(string message) => 
        new DtoValidationResult(false, new[] { new ResultFailureDetail(message) });
}

public class TransactionFactoryTests
{
    private readonly IDtoValidator<CNABFactoryParams> _validator;
    private readonly TransactionFactory _transactionFactory;
    private readonly Guid _cnabFileId;

    public TransactionFactoryTests()
    {
        _validator = Substitute.For<IDtoValidator<CNABFactoryParams>>();
        _transactionFactory = new TransactionFactory(_validator);
        _cnabFileId = Guid.NewGuid();
    }

    [Fact]
    public void Create_WhenDataIsNull_ShouldReturnFailureWithAppropriateMessage()
    {
        // Arrange
        CNABFactoryParams? nullData = null;

        // Act
        var result = _transactionFactory.Create(_cnabFileId, nullData!);

        // Assert
        result.Succeeded.Should().BeFalse();
        result.FailureDetails.Should().Contain(x => x.Description != null && x.Description.Contains("Transaction data cannot be null"));
    }

    [Fact]
    public void Create_WhenValidationFails_ShouldReturnFailureWithValidationErrors()
    {
        // Arrange
        var invalidParams = CNABFactoryParamsBuilder.New
            .WithInvalidCPF()
            .Build();

        var validationFailure = ValidationResult.Failed("CPF is invalid");
        _validator.TryValidate(Arg.Any<CNABFactoryParams>()).Returns(validationFailure);

        // Act
        var result = _transactionFactory.Create(_cnabFileId, invalidParams);

        // Assert
        result.Succeeded.Should().BeFalse();
        result.FailureDetails.Should().Contain(x => x.Description != null && x.Description.Contains("CPF is invalid"));
        _validator.Received(1).TryValidate(invalidParams);
    }

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
    public void Create_WhenValidDataProvided_ShouldCreateCorrectTransactionType(TransactionTypes transactionType, Type expectedType)
    {
        // Arrange
        var validParams = CNABFactoryParamsBuilder.New
            .WithValidData()
            .WithTransactionType(transactionType)
            .Build();

        _validator.TryValidate(Arg.Any<CNABFactoryParams>()).Returns(ValidationResult.Success());

        // Act
        var result = _transactionFactory.Create(_cnabFileId, validParams);

        // Assert
        result.Succeeded.Should().BeTrue();
        result.Value.Should().NotBeNull();
        result.Value.Should().BeOfType(expectedType);
        result.Value.CNABFileId.Should().Be(_cnabFileId);
    }

    [Fact]
    public void Create_WhenValidDataProvided_ShouldReturnSuccessResult()
    {
        // Arrange
        var validParams = CNABFactoryParamsBuilder.New
            .WithValidData()
            .Build();

        _validator.TryValidate(Arg.Any<CNABFactoryParams>()).Returns(ValidationResult.Success());

        // Act
        var result = _transactionFactory.Create(_cnabFileId, validParams);

        // Assert
        result.Succeeded.Should().BeTrue();
        result.Value.Should().NotBeNull();
        // Factory's responsibility is only to create the transaction, not to test its properties
        // Transaction properties are tested in TransactionTests
    }

    [Fact]
    public void Create_WhenUnknownTransactionType_ShouldReturnFailureWithErrorMessage()
    {
        // Arrange
        var invalidTypeParams = CNABFactoryParamsBuilder.New
            .WithValidData()
            .WithTransactionType((TransactionTypes)999)
            .Build();

        _validator.TryValidate(Arg.Any<CNABFactoryParams>()).Returns(ValidationResult.Success());

        // Act
        var result = _transactionFactory.Create(_cnabFileId, invalidTypeParams);

        // Assert
        result.Succeeded.Should().BeFalse();
        result.FailureDetails.Should().Contain(x => x.Description != null && x.Description.Contains("Error creating transaction"));
    }

    [Fact]
    public void Create_WhenMultipleTransactionsCreated_ShouldHaveUniqueIds()
    {
        // Arrange
        var validParams = CNABFactoryParamsBuilder.New
            .WithValidData()
            .Build();

        _validator.TryValidate(Arg.Any<CNABFactoryParams>()).Returns(ValidationResult.Success());

        // Act
        var result1 = _transactionFactory.Create(_cnabFileId, validParams);
        var result2 = _transactionFactory.Create(_cnabFileId, validParams);
        var result3 = _transactionFactory.Create(_cnabFileId, validParams);

        // Assert
        result1.Succeeded.Should().BeTrue();
        result2.Succeeded.Should().BeTrue();
        result3.Succeeded.Should().BeTrue();
        
        result1.Value!.Id.Should().NotBe(result2.Value!.Id);
        result2.Value!.Id.Should().NotBe(result3.Value!.Id);
        result1.Value!.Id.Should().NotBe(result3.Value!.Id);
    }

    [Fact]
    public void Constructor_WithoutParameters_ShouldCreateInstanceWithDefaultValidator()
    {
        // Arrange & Act
        var factory = new TransactionFactory();
        var validParams = CNABFactoryParamsBuilder.New
            .WithValidData()
            .Build();

        var result = factory.Create(_cnabFileId, validParams);

        // Assert
        result.Should().NotBeNull();
        // The default constructor uses CNABLineDataDtoValidator
    }

    [Fact]
    public void Create_WhenRandomDataProvided_ShouldCreateValidTransaction()
    {
        // Arrange
        var randomParams = CNABFactoryParamsBuilder.New
            .WithRandomData()
            .Build();

        _validator.TryValidate(Arg.Any<CNABFactoryParams>()).Returns(ValidationResult.Success());

        // Act
        var result = _transactionFactory.Create(_cnabFileId, randomParams);

        // Assert
        result.Succeeded.Should().BeTrue();
        result.Value.Should().NotBeNull();
        result.Value.Id.Should().NotBeEmpty();
        result.Value.CNABFileId.Should().Be(_cnabFileId);
    }
}
