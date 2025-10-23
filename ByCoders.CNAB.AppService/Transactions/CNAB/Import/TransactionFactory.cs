using ByCoders.CNAB.Core.Results;
using ByCoders.CNAB.Domain.Transactions.Models;
using FluentValidation;

namespace ByCoders.CNAB.AppService.Transactions.CNAB.Import;

/// <summary>
/// DTO containing parsed CNAB line data
/// </summary>
public record CNABLineDataDto(TransactionTypes TransactionType, DateOnly Date, decimal Amount, string CPF, string CardNumber, TimeOnly Time, string StoreOwner, string StoreName);


/// <summary>
/// Factory Pattern - Creates the correct Transaction subclass based on type with Result Pattern
/// Uses FluentValidation for data validation
/// </summary>
public class TransactionFactory : ITransactionFactory
{
    private readonly IValidator<CNABLineDataDto> _validator;

    public TransactionFactory(IValidator<CNABLineDataDto> validator)
    {
        _validator = validator;
    }

    // Construtor sem parâmetros para manter compatibilidade com testes existentes
    public TransactionFactory() : this(new CNABLineDataDtoValidator())
    {
    }

    public Result<Transaction> Create(CNABLineDataDto data)
    {
        if (data == null)
            return Result<Transaction>.Failure("Transaction data cannot be null");

        var validationResult = ValidateData(data);
        if (validationResult.IsFailure)
            return Result<Transaction>.Failure(validationResult.Error);

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

    private Result ValidateData(CNABLineDataDto data)
    {
        var validationResult = _validator.Validate(data);

        if (!validationResult.IsValid)
        {
            var errors = string.Join("; ", validationResult.Errors.Select(e => e.ErrorMessage));
            return Result.Failure(errors);
        }

        return Result.Success();
    }
}