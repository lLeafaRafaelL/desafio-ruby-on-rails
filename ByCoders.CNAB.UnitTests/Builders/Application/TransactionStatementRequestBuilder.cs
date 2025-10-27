using ByCoders.CNAB.Application.Transactions.FindTransactions;

namespace ByCoders.CNAB.UnitTests.Builders.Application;

public class TransactionStatementRequestBuilder
{
    private string _storeName = "TestStore";
    private DateTimeOffset _startDate = DateTimeOffset.UtcNow.AddDays(-1);
    private DateTimeOffset _endDate = DateTimeOffset.UtcNow;

    public static TransactionStatementRequestBuilder New => new();

    public TransactionStatementRequestBuilder WithStoreName(string storeName)
    {
        _storeName = storeName;
        return this;
    }

    public TransactionStatementRequestBuilder WithEmptyStoreName()
    {
        _storeName = string.Empty;
        return this;
    }

    public TransactionStatementRequestBuilder WithLongStoreName()
    {
        _storeName = new string('A', 20); // MaxLength is 19
        return this;
    }

    public TransactionStatementRequestBuilder WithValidStoreName()
    {
        _storeName = "Store ABC";
        return this;
    }

    public TransactionStatementRequestBuilder WithStartDate(DateTimeOffset startDate)
    {
        _startDate = startDate;
        return this;
    }

    public TransactionStatementRequestBuilder WithEndDate(DateTimeOffset endDate)
    {
        _endDate = endDate;
        return this;
    }

    public TransactionStatementRequestBuilder WithMinStartDate()
    {
        _startDate = DateTime.MinValue;
        return this;
    }

    public TransactionStatementRequestBuilder WithStartDateAfterEndDate()
    {
        _startDate = DateTimeOffset.UtcNow.AddDays(1);
        _endDate = DateTimeOffset.UtcNow;
        return this;
    }

    public TransactionStatementRequestBuilder WithEndDateBeforeStartDate()
    {
        _startDate = DateTimeOffset.UtcNow;
        _endDate = DateTimeOffset.UtcNow.AddDays(-1);
        return this;
    }

    public TransactionStatementRequestBuilder WithEndDateInPast()
    {
        _startDate = DateTimeOffset.UtcNow.AddDays(-2);
        _endDate = DateTimeOffset.UtcNow.AddDays(-1);
        return this;
    }

    public TransactionStatementRequestBuilder WithPeriodGreaterThanOneDay()
    {
        _startDate = DateTimeOffset.UtcNow.AddDays(-2);
        _endDate = DateTimeOffset.UtcNow;
        return this;
    }

    public TransactionStatementRequestBuilder WithValidPeriod()
    {
        _startDate = DateTimeOffset.UtcNow.AddHours(1);
        _endDate = DateTimeOffset.UtcNow.AddHours(23);
        return this;
    }

    public TransactionStatementRequestBuilder WithExactlyOneDayPeriod()
    {
        _startDate = DateTimeOffset.UtcNow.AddHours(1);
        _endDate = _startDate.AddDays(1);
        return this;
    }

    public TransactionStatementRequestBuilder WithRandomData()
    {
        var random = new Random();
        _storeName = $"Store{random.Next(1, 100)}";
        _startDate = DateTimeOffset.UtcNow.AddHours(random.Next(1, 12));
        _endDate = _startDate.AddHours(random.Next(1, 23));
        return this;
    }

    public TransactionStatementRequest Build()
    {
        return new TransactionStatementRequest(_storeName, _startDate, _endDate);
    }

    public static implicit operator TransactionStatementRequest(TransactionStatementRequestBuilder builder)
    {
        return builder.Build();
    }
}
