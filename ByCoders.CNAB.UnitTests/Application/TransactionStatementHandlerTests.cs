using ByCoders.CNAB.Application.Transactions.FindTransactions;
using ByCoders.CNAB.Core;
using ByCoders.CNAB.Core.Validators;
using ByCoders.CNAB.Domain.Transactions;
using ByCoders.CNAB.Domain.Transactions.Models;
using ByCoders.CNAB.UnitTests.Builders.Application;
using ByCoders.CNAB.UnitTests.Builders.Domain;
using FluentAssertions;
using Microsoft.Extensions.Caching.Memory;
using NSubstitute;
using Xunit;

namespace ByCoders.CNAB.UnitTests.Application;

public class TransactionStatementHandlerTests
{
    private readonly ITransactionRepository _repository;
    private readonly IDtoValidator<TransactionStatementRequest> _validator;
    private readonly IMemoryCache _memoryCache;
    private readonly TransactionStatementHandler _handler;

    public TransactionStatementHandlerTests()
    {
        _repository = Substitute.For<ITransactionRepository>();
        _validator = Substitute.For<IDtoValidator<TransactionStatementRequest>>();
        _memoryCache = new MemoryCache(new MemoryCacheOptions());
        _handler = new TransactionStatementHandler(_repository, _validator, _memoryCache);
    }

    [Fact]
    public async Task HandleAsync_WhenValidationFails_ShouldReturnUnprocessableResult()
    {
        // Arrange
        var request = TransactionStatementRequestBuilder.New
            .WithEmptyStoreName()
            .WithValidPeriod()
            .Build();

        _validator.TryValidate(request)
            .Returns(ValidationResult.Failed("Store name is required"));

        // Act
        var result = await _handler.HandleAsync(request, CancellationToken.None);

        // Assert
        result.Succeeded.Should().BeFalse();
        result.Status.Should().Be(RequestHandlerStatus.Unprocessable);
        result.FailureDetails.Should().Contain(x => x.Description == "Store name is required");
        await _repository.DidNotReceive().FindBy(
            Arg.Any<string>(),
            Arg.Any<DateTimeOffset>(),
            Arg.Any<DateTimeOffset>(),
            Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task HandleAsync_WhenNoTransactionsFound_ShouldReturnNoContent()
    {
        // Arrange
        var request = TransactionStatementRequestBuilder.New
            .WithValidStoreName()
            .WithValidPeriod()
            .Build();

        _validator.TryValidate(request).Returns(ValidationResult.Success());

        _repository.FindBy(
                request.StoreName,
                request.StartDate,
                request.EndDate,
                Arg.Any<CancellationToken>())
            .Returns(Task.FromResult<IEnumerable<Transaction>>(null!));

        // Act
        var result = await _handler.HandleAsync(request, CancellationToken.None);

        // Assert
        result.Succeeded.Should().BeTrue();
        result.Status.Should().Be(RequestHandlerStatus.NoContent);
        result.Value.Should().BeNull();
    }

    [Fact]
    public async Task HandleAsync_WhenTransactionsFound_ShouldReturnSuccessWithCorrectData()
    {
        // Arrange
        var request = TransactionStatementRequestBuilder.New
            .WithValidStoreName()
            .WithValidPeriod()
            .Build();

        var transactions = new List<Transaction>
        {
            SaleBuilder.New.WithAmountCNAB(10000m).Build(),
            DebitBuilder.New.WithAmountCNAB(5000m).Build(),
            CreditBuilder.New.WithAmountCNAB(15000m).Build()
        };

        _validator.TryValidate(request).Returns(ValidationResult.Success());

        _repository.FindBy(
                request.StoreName,
                request.StartDate,
                request.EndDate,
                Arg.Any<CancellationToken>())
            .Returns(Task.FromResult<IEnumerable<Transaction>>(transactions));

        // Act
        var result = await _handler.HandleAsync(request, CancellationToken.None);

        // Assert
        result.Succeeded.Should().BeTrue();
        result.Status.Should().Be(RequestHandlerStatus.OK);
        result.Value.Should().NotBeNull();
        result.Value!.TotalTrsanctions.Should().Be(3);
        result.Value.Transactions.Should().HaveCount(3);
        result.Value.AccumulatedValue.Should().Be(300m);

        _repository.Received(1).FindBy(
            request.StoreName,
            request.StartDate,
            request.EndDate,
            Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task HandleAsync_WhenTransactionsFoundOnCache_ShouldNotFetchFromRepository()
    {
        // Arrange
        var request = TransactionStatementRequestBuilder.New
            .WithValidStoreName()
            .WithValidPeriod()
            .Build();

        var transactions = new List<Transaction>
        {
            SaleBuilder.New.WithAmountCNAB(10000m).Build(),
            DebitBuilder.New.WithAmountCNAB(5000m).Build(),
            CreditBuilder.New.WithAmountCNAB(15000m).Build()
        };

        var response = new TransactionStatementResponse(request.StartDate, request.EndDate, transactions);

        _validator.TryValidate(request).Returns(ValidationResult.Success());

        _memoryCache.Set(request.IdempotencyKey, response, TimeSpan.FromMinutes(1));

        // Act
        var result = await _handler.HandleAsync(request, CancellationToken.None);

        // Assert
        result.Succeeded.Should().BeTrue();
        result.Status.Should().Be(RequestHandlerStatus.OK);
        result.Value.Should().NotBeNull();
        result.Value!.TotalTrsanctions.Should().Be(3);
        result.Value.Transactions.Should().HaveCount(3);
        result.Value.AccumulatedValue.Should().Be(300m);

        _repository.DidNotReceive().FindBy(
            Arg.Any<string>(),
            Arg.Any<DateTimeOffset>(),
            Arg.Any<DateTimeOffset>(),
            Arg.Any<CancellationToken>());

        _memoryCache.TryGetValue(request.IdempotencyKey, out TransactionStatementResponse resultCache);
        resultCache.Should().NotBeNull();
        resultCache.Should().Be(response);
    }

    [Fact]
    public async Task HandleAsync_ShouldCalculateAccumulatedValueCorrectly()
    {
        // Arrange
        var request = TransactionStatementRequestBuilder.New
            .WithValidStoreName()
            .WithValidPeriod()
            .Build();

        var transactions = new List<Transaction>
        {
            SaleBuilder.New.WithAmountCNAB(10050m).Build(),    // 100.50
            DebitBuilder.New.WithAmountCNAB(20075m).Build(),   // 200.75
            CreditBuilder.New.WithAmountCNAB(30025m).Build()   // 300.25
        };

        _validator.TryValidate(request).Returns(ValidationResult.Success());

        _repository.FindBy(
                request.StoreName,
                request.StartDate,
                request.EndDate,
                Arg.Any<CancellationToken>())
            .Returns(Task.FromResult<IEnumerable<Transaction>>(transactions));

        // Act
        var result = await _handler.HandleAsync(request, CancellationToken.None);

        // Assert
        result.Succeeded.Should().BeTrue();
        result.Value.Should().NotBeNull();
        result.Value!.AccumulatedValue.Should().Be(601.50m); // 100.50 + 200.75 + 300.25
    }

    [Fact]
    public async Task HandleAsync_WithSingleTransaction_ShouldReturnCorrectCount()
    {
        // Arrange
        var request = TransactionStatementRequestBuilder.New
            .WithValidStoreName()
            .WithValidPeriod()
            .Build();

        var transactions = new List<Transaction>
        {
            SaleBuilder.New.WithAmountCNAB(10000m).Build()
        };

        _validator.TryValidate(request).Returns(ValidationResult.Success());

        _repository.FindBy(
                request.StoreName,
                request.StartDate,
                request.EndDate,
                Arg.Any<CancellationToken>())
            .Returns(Task.FromResult<IEnumerable<Transaction>>(transactions));

        // Act
        var result = await _handler.HandleAsync(request, CancellationToken.None);

        // Assert
        result.Succeeded.Should().BeTrue();
        result.Value.Should().NotBeNull();
        result.Value!.TotalTrsanctions.Should().Be(1);
        result.Value.Transactions.Should().HaveCount(1);
        result.Value.AccumulatedValue.Should().Be(100m);
    }

    [Fact]
    public async Task HandleAsync_WithZeroValueTransactions_ShouldCalculateCorrectly()
    {
        // Arrange
        var request = TransactionStatementRequestBuilder.New
            .WithValidStoreName()
            .WithValidPeriod()
            .Build();

        var transactions = new List<Transaction>
        {
            SaleBuilder.New.WithZeroAmount().Build(),
            DebitBuilder.New.WithZeroAmount().Build()
        };

        _validator.TryValidate(request).Returns(ValidationResult.Success());

        _repository.FindBy(
                request.StoreName,
                request.StartDate,
                request.EndDate,
                Arg.Any<CancellationToken>())
            .Returns(Task.FromResult<IEnumerable<Transaction>>(transactions));

        // Act
        var result = await _handler.HandleAsync(request, CancellationToken.None);

        // Assert
        result.Succeeded.Should().BeTrue();
        result.Value.Should().NotBeNull();
        result.Value!.TotalTrsanctions.Should().Be(2);
        result.Value.AccumulatedValue.Should().Be(0m);
    }

    [Fact]
    public async Task HandleAsync_WithLargeNumberOfTransactions_ShouldProcessSuccessfully()
    {
        // Arrange
        var request = TransactionStatementRequestBuilder.New
            .WithValidStoreName()
            .WithValidPeriod()
            .Build();

        var transactions = Enumerable.Range(1, 1000)
            .Select(_ => SaleBuilder.New.WithAmountCNAB(10000m).Build() as Transaction)
            .ToList();

        _validator.TryValidate(request).Returns(ValidationResult.Success());

        _repository.FindBy(
                request.StoreName,
                request.StartDate,
                request.EndDate,
                Arg.Any<CancellationToken>())
            .Returns(Task.FromResult<IEnumerable<Transaction>>(transactions));

        // Act
        var result = await _handler.HandleAsync(request, CancellationToken.None);

        // Assert
        result.Succeeded.Should().BeTrue();
        result.Value.Should().NotBeNull();
        result.Value!.TotalTrsanctions.Should().Be(1000);
        result.Value.Transactions.Should().HaveCount(1000);
        result.Value.AccumulatedValue.Should().Be(100000m); // 1000 * 100
    }

    [Fact]
    public async Task HandleAsync_ShouldPassCorrectParametersToRepository()
    {
        // Arrange
        var storeName = "TestStore123";
        var startDate = DateTimeOffset.UtcNow.AddHours(1);
        var endDate = startDate.AddHours(12);

        var request = TransactionStatementRequestBuilder.New
            .WithStoreName(storeName)
            .WithStartDate(startDate)
            .WithEndDate(endDate)
            .Build();

        var transactions = new List<Transaction>
        {
            SaleBuilder.New.Build()
        };

        _validator.TryValidate(request).Returns(ValidationResult.Success());

        _repository.FindBy(
                Arg.Any<string>(),
                Arg.Any<DateTimeOffset>(),
                Arg.Any<DateTimeOffset>(),
                Arg.Any<CancellationToken>())
            .Returns(Task.FromResult<IEnumerable<Transaction>>(transactions));

        // Act
        var result = await _handler.HandleAsync(request, CancellationToken.None);

        // Assert
        result.Succeeded.Should().BeTrue();
        await _repository.Received(1).FindBy(
            storeName,
            startDate,
            endDate,
            Arg.Any<CancellationToken>());
    }


    [Fact]
    public void Constructor_WhenRepositoryIsNull_ShouldThrowArgumentNullException()
    {
        // Arrange & Act
        var act = () => new TransactionStatementHandler(null!, _validator, _memoryCache);

        // Assert
        act.Should().Throw<ArgumentNullException>()
            .WithParameterName("repository");
    }

    [Fact]
    public void Constructor_WhenValidatorIsNull_ShouldThrowArgumentNullException()
    {
        // Arrange & Act
        var act = () => new TransactionStatementHandler(_repository, null, _memoryCache);

        // Assert
        act.Should().Throw<ArgumentNullException>()
            .WithParameterName("validator");
    }

    [Fact]
    public async Task HandleAsync_WithMixedTransactionTypes_ShouldIncludeAllInResponse()
    {
        // Arrange
        var request = TransactionStatementRequestBuilder.New
            .WithValidStoreName()
            .WithValidPeriod()
            .Build();

        var transactions = new List<Transaction>
        {
            SaleBuilder.New.WithAmountCNAB(10000m).Build(),
            DebitBuilder.New.WithAmountCNAB(5000m).Build(),
            CreditBuilder.New.WithAmountCNAB(8000m).Build(),
            BankSlipBuilder.New.WithAmountCNAB(12000m).Build(),
            FundingBuilder.New.WithAmountCNAB(15000m).Build()
        };

        _validator.TryValidate(request).Returns(ValidationResult.Success());

        _repository.FindBy(
                request.StoreName,
                request.StartDate,
                request.EndDate,
                Arg.Any<CancellationToken>())
            .Returns(Task.FromResult<IEnumerable<Transaction>>(transactions));

        // Act
        var result = await _handler.HandleAsync(request, CancellationToken.None);

        // Assert
        result.Succeeded.Should().BeTrue();
        result.Value.Should().NotBeNull();
        result.Value!.TotalTrsanctions.Should().Be(5);
        result.Value.Transactions.Should().HaveCount(5);
        result.Value.AccumulatedValue.Should().Be(-40);
    }

    [Fact]
    public async Task HandleAsync_ResponseShouldContainCorrectDateRange()
    {
        // Arrange
        var startDate = DateTimeOffset.UtcNow.AddHours(2);
        var endDate = startDate.AddHours(20);

        var request = TransactionStatementRequestBuilder.New
            .WithValidStoreName()
            .WithStartDate(startDate)
            .WithEndDate(endDate)
            .Build();

        var transactions = new List<Transaction>
        {
            SaleBuilder.New.Build()
        };

        _validator.TryValidate(request).Returns(ValidationResult.Success());

        _repository.FindBy(
                request.StoreName,
                request.StartDate,
                request.EndDate,
                Arg.Any<CancellationToken>())
            .Returns(Task.FromResult<IEnumerable<Transaction>>(transactions));

        // Act
        var result = await _handler.HandleAsync(request, CancellationToken.None);

        // Assert
        result.Succeeded.Should().BeTrue();
        result.Value.Should().NotBeNull();
        result.Value!.StartDate.Should().Be(startDate);
        result.Value.EndDate.Should().Be(endDate);
    }
}
