using ByCoders.CNAB.Core;
using ByCoders.CNAB.Core.Results;
using ByCoders.CNAB.Core.Validators;
using ByCoders.CNAB.Domain.Transactions.Models;

namespace ByCoders.CNAB.Application.Transactions;

/// <summary>
/// Factory Pattern - Creates the correct Transaction subclass based on type with Result Pattern
/// Uses FluentValidation for data validation
/// </summary>
public class TransactionFactory : ITransactionFactory
{
    private readonly IDtoValidator<CNABFactoryParams> _validator;

    public TransactionFactory(IDtoValidator<CNABFactoryParams> validator)
    {
        _validator = validator;
    }

    // Construtor sem parâmetros para manter compatibilidade com testes existentes
    public TransactionFactory() : this(new CNABFactoryParamsValidator())
    {
    }

    public Result<Transaction> Create(Guid cnabFileId, CNABFactoryParams data)
    {
        if (data == null)
            return Result<Transaction>.Failure("Transaction data cannot be null");

        var validationResult = _validator.TryValidate(data);
        if (validationResult.IsValid is false)
            return Result<Transaction>.Failure(validationResult.FailureDetails);

        try
        {
            var beneficiary = new Beneficiary(data.CPF);
            var card = new Card(data.CardNumber);
            var store = new Store(data.StoreName, data.StoreOwner);

            Transaction transaction = data.TransactionType switch
            {
                TransactionTypes.Debit => new Debit(cnabFileId, data.Date, data.Time, data.Amount, beneficiary, card, store),
                TransactionTypes.BankSlip => new BankSlip(cnabFileId, data.Date, data.Time, data.Amount, beneficiary, card, store),
                TransactionTypes.Funding => new Funding(cnabFileId, data.Date, data.Time, data.Amount, beneficiary, card, store),
                TransactionTypes.Credit => new Credit(cnabFileId, data.Date, data.Time, data.Amount, beneficiary, card, store),
                TransactionTypes.LoanReceipt => new LoanReceipt(cnabFileId, data.Date, data.Time, data.Amount, beneficiary, card, store),
                TransactionTypes.Sales => new Sale(cnabFileId, data.Date, data.Time, data.Amount, beneficiary, card, store),
                TransactionTypes.TEDReceipt => new TEDReceipt(cnabFileId, data.Date, data.Time, data.Amount, beneficiary, card, store),
                TransactionTypes.DOCReceipt => new DOCReceipt(cnabFileId, data.Date, data.Time, data.Amount, beneficiary, card, store),
                TransactionTypes.Rent => new Rent(cnabFileId, data.Date, data.Time, data.Amount, beneficiary, card, store),
                _ => throw new ArgumentException($"Unknown transaction type: {data.TransactionType}")
            };

            return Result<Transaction>.Success(transaction);
        }
        catch (Exception ex)
        {
            return Result<Transaction>.Failure($"Error creating transaction: {ex.Message}");
        }
    }
}

/// <summary>
/// DTO containing parsed CNAB line data
/// </summary>
public record CNABFactoryParams(TransactionTypes TransactionType, DateOnly Date, decimal Amount, string CPF, string CardNumber, TimeOnly Time, string StoreOwner, string StoreName)
    : Dto;