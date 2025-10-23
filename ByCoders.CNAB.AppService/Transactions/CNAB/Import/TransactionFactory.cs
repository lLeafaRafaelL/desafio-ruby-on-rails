using ByCoders.CNAB.Core.Results;
using ByCoders.CNAB.Domain.Transactions.Models;

namespace ByCoders.CNAB.AppService.Transactions.CNAB.Import;

/// <summary>
/// DTO containing parsed CNAB line data
/// </summary>
public record CNABLineDataDto(TransactionTypes TransactionType, DateOnly Date, decimal Amount, string CPF, string CardNumber, TimeOnly Time, string StoreOwner, string StoreName);


/// <summary>
/// Factory Pattern - Creates the correct Transaction subclass based on type with Result Pattern
/// </summary>
public class TransactionFactory
{
    public Result<Transaction> Create(CNABLineDataDto data)
    {
        var validationError = ValidateData(data);
        if (!string.IsNullOrEmpty(validationError))
            return Result<Transaction>.Failure(validationError);

        try
        {
            var beneficiary = new Beneficiary(data.CPF);
            var card = new Card(data.CardNumber);
            var store = new Store(data.StoreName, data.StoreOwner);

            Transaction transaction = data.TransactionType switch
            {
                TransactionTypes.Debit => new Debit(data.Date, data.Time, data.Amount, beneficiary, card, store),
                TransactionTypes.BankSlip => new BankSlip(data.Date, data.Time, data.Amount, beneficiary, card, store),
                TransactionTypes.Funding => new Funding(data.Date, data.Time, data.Amount, beneficiary, card, store),
                TransactionTypes.Credit => new Credit(data.Date, data.Time, data.Amount, beneficiary, card, store),
                TransactionTypes.LoanReceipt => new LoanReceipt(data.Date, data.Time, data.Amount, beneficiary, card, store),
                TransactionTypes.Sales => new Sale(data.Date, data.Time, data.Amount, beneficiary, card, store),
                TransactionTypes.TEDReceipt => new TEDReceipt(data.Date, data.Time, data.Amount, beneficiary, card, store),
                TransactionTypes.DOCReceipt => new DOCReceipt(data.Date, data.Time, data.Amount, beneficiary, card, store),
                TransactionTypes.Rent => new Rent(data.Date, data.Time, data.Amount, beneficiary, card, store),
                _ => throw new ArgumentException($"Unknown transaction type: {data.TransactionType}")
            };

            return Result<Transaction>.Success(transaction);
        }
        catch (Exception ex)
        {
            return Result<Transaction>.Failure($"Error creating transaction: {ex.Message}");
        }
    }

    private string ValidateData(CNABLineDataDto data)
    {
        if (data == null)
            return "Transaction data cannot be null";

        if (string.IsNullOrWhiteSpace(data.CPF))
            return "CPF cannot be empty";

        if (string.IsNullOrWhiteSpace(data.CardNumber))
            return "Card number cannot be empty";

        if (string.IsNullOrWhiteSpace(data.StoreName))
            return "Store name cannot be empty";

        if (string.IsNullOrWhiteSpace(data.StoreOwner))
            return "Store owner cannot be empty";

        if (data.Amount < 0)
            return "Amount cannot be negative";

        return string.Empty; // No validation errors
    }
}