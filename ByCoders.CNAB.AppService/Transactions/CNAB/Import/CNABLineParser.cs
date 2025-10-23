using ByCoders.CNAB.Core.Results;
using ByCoders.CNAB.Domain.Transactions.Models;

namespace ByCoders.CNAB.AppService.Transactions.CNAB.Import;

/// <summary>
/// Parser for CNAB format lines - Strategy Pattern with Result Pattern
/// Responsible for extracting data from fixed-position string format
/// </summary>
public class CNABLineParser
{
    public Result<CNABLineDataDto> Parse(string line)
    {
        if (string.IsNullOrWhiteSpace(line))
            return Result<CNABLineDataDto>.Failure("CNAB line cannot be empty");

        if (line.Length < 81)
            return Result<CNABLineDataDto>.Failure($"CNAB line must be at least 81 characters. Got {line.Length}");

        try
        {
            var data = new CNABLineDataDto
            (
                TransactionType: ParseTransactionType(line.Substring(0, 1)),
                Date: ParseDate(line.Substring(1, 8)),
                Amount: ParseAmount(line.Substring(9, 10)),
                CPF: line.Substring(19, 11).Trim(),
                CardNumber: line.Substring(30, 12).Trim(),
                Time: ParseTime(line.Substring(42, 6)),
                StoreOwner: line.Substring(48, 14).Trim(),
                StoreName: line.Substring(62, 19).Trim()
            );

            return Result<CNABLineDataDto>.Success(data);
        }
        catch (Exception ex)
        {
            return Result<CNABLineDataDto>.Failure($"Error parsing CNAB line: {ex.Message}");
        }
    }

    private TransactionTypes ParseTransactionType(string type)
    {
        if (!int.TryParse(type, out int typeId) || typeId < 1 || typeId > 9)
            throw new ArgumentException($"Invalid transaction type: {type}");

        return (TransactionTypes)typeId;
    }

    private DateOnly ParseDate(string dateString)
    {
        // Format: YYYYMMDD
        if (dateString.Length != 8)
            throw new ArgumentException($"Invalid date format: {dateString}");

        int year = int.Parse(dateString.Substring(0, 4));
        int month = int.Parse(dateString.Substring(4, 2));
        int day = int.Parse(dateString.Substring(6, 2));

        return new DateOnly(year, month, day);
    }

    private decimal ParseAmount(string amountString)
    {
        // Amount needs to be divided by 100 to normalize
        if (!long.TryParse(amountString, out long amount))
            throw new ArgumentException($"Invalid amount format: {amountString}");

        return amount;
    }

    private TimeOnly ParseTime(string timeString)
    {
        // Format: HHMMSS
        if (timeString.Length != 6)
            throw new ArgumentException($"Invalid time format: {timeString}");

        int hour = int.Parse(timeString.Substring(0, 2));
        int minute = int.Parse(timeString.Substring(2, 2));
        int second = int.Parse(timeString.Substring(4, 2));

        return new TimeOnly(hour, minute, second);
    }
}
