using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ByCoders.CNAB.Core;
using ByCoders.CNAB.Core.Results;
using ByCoders.CNAB.Domain.Transactions;
using ByCoders.CNAB.Domain.Transactions.Models;

namespace ByCoders.CNAB.AppService.Transactions.CNAB.Import;

/// <summary>
/// Handler for importing CNAB files
/// Uses bulk insert for high performance when saving multiple transactions
/// </summary>
public class ImportCNABRequestHandler : IRequestHandler<ImportCNABRequest, ImportCNABResponse>
{
    private readonly ITransactionRepository _repository;
    private readonly ITransactionFactory _transactionFactory;
    private readonly CNABLineParser _parser;

    public ImportCNABRequestHandler(
        ITransactionRepository repository,
        ITransactionFactory transactionFactory,
        CNABLineParser parser)
    {
        _repository = repository;
        _transactionFactory = transactionFactory;
        _parser = parser;
    }

    public async Task<ImportCNABResponse> Handle(ImportCNABRequest request, CancellationToken cancellationToken)
    {
        var transactions = new List<Transaction>();
        var errors = new List<string>();

        using (var stream = request.CNABFile.OpenReadStream())
        {
            using (var memoryStream = new MemoryStream())
            {
                await stream.CopyToAsync(memoryStream, cancellationToken);
                memoryStream.Position = 0; // Reset position to read from start

                using (var reader = new StreamReader(memoryStream))
                {
                    string line;
                    var lines = new List<string>();

                    while ((line = await reader.ReadLineAsync(cancellationToken)) != null)
                    {
                        if (!string.IsNullOrWhiteSpace(line))
                        {
                            lines.Add(line);
                        }
                    }

                    // Process lines sequentially to maintain line numbers for error reporting
                    int lineNumber = 0;
                    foreach (var currentLine in lines)
                    {
                        cancellationToken.ThrowIfCancellationRequested();
                        lineNumber++;
                        
                        // Parse line
                        var parseResult = _parser.Parse(currentLine);
                        if (parseResult.IsFailure)
                        {
                            errors.Add($"Line {lineNumber}: {parseResult.Error}");
                            continue; // Skip this line, continue processing
                        }

                        // Create transaction
                        var createResult = _transactionFactory.Create(parseResult.Value!);
                        if (createResult.IsFailure)
                        {
                            errors.Add($"Line {lineNumber}: {createResult.Error}");
                            continue; // Skip this line, continue processing
                        }

                        transactions.Add(createResult.Value!);
                    }
                }
            }
        }

        // Bulk insert transactions for high performance
        if (transactions.Any())
        {
            await _repository.BulkInsertAsync(transactions, cancellationToken);
        }

        return new ImportCNABResponse
        {
            Success = errors.Count == 0,
            TransactionsImported = transactions.Count,
            Errors = errors
        };
    }
}
