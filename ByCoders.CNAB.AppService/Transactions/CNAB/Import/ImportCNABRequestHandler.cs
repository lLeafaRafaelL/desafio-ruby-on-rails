using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ByCoders.CNAB.Core;
using ByCoders.CNAB.Core.Results;
using ByCoders.CNAB.Domain.Transactions.Models;

namespace ByCoders.CNAB.AppService.Transactions.CNAB.Import;

internal class ImportCNABRequestHandler : IRequestHandler<ImportCNABRequest, ImportCNABResponse>
{
    public async Task<ImportCNABResponse> Handle(ImportCNABRequest request, CancellationToken cancellationToken)
    {
        var transactions = new List<Transaction>();
        var errors = new List<string>();
        var factory = new TransactionFactory();
        var parser = new CNABLineParser();

        using (var stream = request.CNABFile.OpenReadStream())
        {
            using (var memoryStream = new MemoryStream())
            {
                await stream.CopyToAsync(memoryStream);
                memoryStream.Position = 0; // Reset position to read from start

                using (var reader = new StreamReader(memoryStream))
                {
                    string line;
                    var lines = new List<string>();

                    while ((line = await reader.ReadLineAsync()) != null)
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
                        lineNumber++;
                        
                        // Parse line
                        var parseResult = parser.Parse(currentLine);
                        if (parseResult.IsFailure)
                        {
                            errors.Add($"Line {lineNumber}: {parseResult.Error}");
                            continue; // Skip this line, continue processing
                        }

                        // Create transaction
                        var createResult = factory.Create(parseResult.Value!);
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

        return new ImportCNABResponse
        {
            Success = errors.Count == 0,
            TransactionsImported = transactions.Count,
            Errors = errors
        };
    }
}
