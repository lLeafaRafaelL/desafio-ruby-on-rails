using ByCoders.CNAB.Application.Transactions.FindTransactions;
using ByCoders.CNAB.UnitTests.Builders.Application;
using FluentAssertions;
using Xunit;

namespace ByCoders.CNAB.UnitTests.Application;

public class TransactionStatementRequestValidatorTests
{
    private readonly TransactionStatementRequestValidator _validator;

    public TransactionStatementRequestValidatorTests()
    {
        _validator = new TransactionStatementRequestValidator();
    }

    [Fact]
    public void Validate_WhenRequestIsValid_ShouldReturnSuccess()
    {
        // Arrange
        var request = TransactionStatementRequestBuilder.New
            .WithValidStoreName()
            .WithValidPeriod()
            .Build();

        // Act
        var result = _validator.TryValidate(request);

        // Assert
        result.IsValid.Should().BeTrue();
        result.FailureDetails.Should().BeEmpty();
    }

    [Fact]
    public void Validate_WhenStoreNameIsEmpty_ShouldReturnFailure()
    {
        // Arrange
        var request = TransactionStatementRequestBuilder.New
            .WithEmptyStoreName()
            .WithValidPeriod()
            .Build();

        // Act
        var result = _validator.TryValidate(request);

        // Assert
        result.IsValid.Should().BeFalse();
        result.FailureDetails.Should().Contain(x =>
            x.PropertyName == nameof(TransactionStatementRequest.StoreName));
    }

    [Fact]
    public void Validate_WhenStoreNameExceedsMaxLength_ShouldReturnFailure()
    {
        // Arrange
        var request = TransactionStatementRequestBuilder.New
            .WithLongStoreName()
            .WithValidPeriod()
            .Build();

        // Act
        var result = _validator.TryValidate(request);

        // Assert
        result.IsValid.Should().BeFalse();
        result.FailureDetails.Should().Contain(x =>
            x.PropertyName == nameof(TransactionStatementRequest.StoreName));
    }

    [Fact]
    public void Validate_WhenStartDateIsMinValue_ShouldReturnFailure()
    {
        // Arrange
        var request = TransactionStatementRequestBuilder.New
            .WithValidStoreName()
            .WithMinStartDate()
            .WithEndDate(DateTimeOffset.UtcNow.AddDays(1))
            .Build();

        // Act
        var result = _validator.TryValidate(request);

        // Assert
        result.IsValid.Should().BeFalse();
        result.FailureDetails.Should().Contain(x =>
            x.PropertyName == nameof(TransactionStatementRequest.StartDate));
    }

    [Fact]
    public void Validate_WhenStartDateIsAfterEndDate_ShouldReturnFailure()
    {
        // Arrange
        var request = TransactionStatementRequestBuilder.New
            .WithValidStoreName()
            .WithStartDateAfterEndDate()
            .Build();

        // Act
        var result = _validator.TryValidate(request);

        // Assert
        result.IsValid.Should().BeFalse();
        result.FailureDetails.Should().Contain(x =>
            x.PropertyName == nameof(TransactionStatementRequest.StartDate));
    }

    [Fact]
    public void Validate_WhenEndDateIsBeforeStartDate_ShouldReturnFailure()
    {
        // Arrange
        var request = TransactionStatementRequestBuilder.New
            .WithValidStoreName()
            .WithEndDateBeforeStartDate()
            .Build();

        // Act
        var result = _validator.TryValidate(request);

        // Assert
        result.IsValid.Should().BeFalse();
        result.FailureDetails.Should().Contain(x =>
            x.PropertyName == nameof(TransactionStatementRequest.EndDate) ||
            x.PropertyName == nameof(TransactionStatementRequest.StartDate));
    }

    [Fact]
    public void Validate_WhenEndDateIsInPast_ShouldReturnFailure()
    {
        // Arrange
        var request = TransactionStatementRequestBuilder.New
            .WithValidStoreName()
            .WithEndDateInPast()
            .Build();

        // Act
        var result = _validator.TryValidate(request);

        // Assert
        result.IsValid.Should().BeFalse();
        result.FailureDetails.Should().Contain(x =>
            x.PropertyName == nameof(TransactionStatementRequest.EndDate));
    }

    [Fact]
    public void Validate_WhenPeriodExceedsOneDay_ShouldReturnFailure()
    {
        // Arrange
        var request = TransactionStatementRequestBuilder.New
            .WithValidStoreName()
            .WithPeriodGreaterThanOneDay()
            .Build();

        // Act
        var result = _validator.TryValidate(request);

        // Assert
        result.IsValid.Should().BeFalse();
        result.FailureDetails.Should().Contain(x =>
            x.Description != null && x.Description.Contains("maximum consultation period"));
    }

    [Fact]
    public void Validate_WhenPeriodIsExactlyOneDay_ShouldReturnSuccess()
    {
        // Arrange
        var request = TransactionStatementRequestBuilder.New
            .WithValidStoreName()
            .WithExactlyOneDayPeriod()
            .Build();

        // Act
        var result = _validator.TryValidate(request);

        // Assert
        result.IsValid.Should().BeTrue();
    }

    [Theory]
    [InlineData("Store A")]
    [InlineData("Store123")]
    [InlineData("ABC")]
    [InlineData("1234567890123456789")] // 19 characters (max)
    public void Validate_WithValidStoreNames_ShouldReturnSuccess(string storeName)
    {
        // Arrange
        var request = TransactionStatementRequestBuilder.New
            .WithStoreName(storeName)
            .WithValidPeriod()
            .Build();

        // Act
        var result = _validator.TryValidate(request);

        // Assert
        result.IsValid.Should().BeTrue();
    }

    [Fact]
    public void Validate_WithRandomValidData_ShouldReturnSuccess()
    {
        // Arrange
        var request = TransactionStatementRequestBuilder.New
            .WithRandomData()
            .Build();

        // Act
        var result = _validator.TryValidate(request);

        // Assert
        result.IsValid.Should().BeTrue();
    }

    [Fact]
    public void Validate_WhenMultipleValidationsFail_ShouldReturnAllErrors()
    {
        // Arrange
        var request = TransactionStatementRequestBuilder.New
            .WithEmptyStoreName()
            .WithStartDateAfterEndDate()
            .WithEndDateInPast()
            .Build();

        // Act
        var result = _validator.TryValidate(request);

        // Assert
        result.IsValid.Should().BeFalse();
        result.FailureDetails.Should().HaveCountGreaterThan(1);
    }

    [Fact]
    public void Validate_WithShortValidPeriod_ShouldReturnSuccess()
    {
        // Arrange
        var startDate = DateTimeOffset.UtcNow.AddHours(1);
        var endDate = startDate.AddHours(1); // Just 1 hour period

        var request = TransactionStatementRequestBuilder.New
            .WithValidStoreName()
            .WithStartDate(startDate)
            .WithEndDate(endDate)
            .Build();

        // Act
        var result = _validator.TryValidate(request);

        // Assert
        result.IsValid.Should().BeTrue();
    }

    [Fact]
    public void Validate_WithPeriodJustOverOneDay_ShouldReturnFailure()
    {
        // Arrange
        var startDate = DateTimeOffset.UtcNow.AddHours(1);
        var endDate = startDate.AddDays(1).AddMinutes(1); // Just over 1 day

        var request = TransactionStatementRequestBuilder.New
            .WithValidStoreName()
            .WithStartDate(startDate)
            .WithEndDate(endDate)
            .Build();

        // Act
        var result = _validator.TryValidate(request);

        // Assert
        result.IsValid.Should().BeFalse();
        result.FailureDetails.Should().Contain(x =>
            x.Description != null && x.Description.Contains("maximum consultation period"));
    }
}
