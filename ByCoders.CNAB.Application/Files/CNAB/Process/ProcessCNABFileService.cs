using ByCoders.CNAB.Application.Files.CNAB.Parsers;
using ByCoders.CNAB.Application.Transactions;
using ByCoders.CNAB.Core.Results;
using ByCoders.CNAB.Domain.Files;
using ByCoders.CNAB.Domain.Files.Models;
using ByCoders.CNAB.Domain.Transactions;
using ByCoders.CNAB.Domain.Transactions.Models;
using Microsoft.Extensions.Logging;

namespace ByCoders.CNAB.Application.Files.CNAB.Process;

/// <summary>
/// Processes all pending files (Status = Uploaded)
/// </summary>

public class ProcessCNABFileService : IProcessCNABFileService
{
    private readonly ICNABFileRepository _fileRepository;
    private readonly ITransactionRepository _transactionRepository;
    private readonly IFileStorageService _fileStorage;
    private readonly ICNABLineParser _parser;
    private readonly ITransactionFactory _factory;
    private readonly ILogger<ProcessCNABFileService> _logger;

    public ProcessCNABFileService(ICNABFileRepository fileRepository, ITransactionRepository transactionRepository, IFileStorageService fileStorage, ICNABLineParser parser, ITransactionFactory factory, ILogger<ProcessCNABFileService> logger)
    {
        _fileRepository = fileRepository;
        _transactionRepository = transactionRepository;
        _fileStorage = fileStorage;
        _parser = parser;
        _factory = factory;
        _logger = logger;
    }

    public async Task<int> ProcessPendingFilesAsync(CancellationToken cancellationToken)
    {
        var pendingFiles = await _fileRepository.FindByStatusAsync(CNABFileStatus.Uploaded, cancellationToken);
        var filesList = pendingFiles.ToList();

        if (!filesList.Any())
        {
            _logger.LogDebug("No pending files to process");
            return 0;
        }

        _logger.LogInformation("Found {Count} pending files to process", filesList.Count);

        int processedCount = 0;

        foreach (var file in filesList)
        {
            if (cancellationToken.IsCancellationRequested)
                break;

            var result = await ProcessFileAsync(file, cancellationToken);
            if (result.Succeeded)
                processedCount++;
        }

        return processedCount;
    }


    /// <summary>
    /// Processes a specific CNAB file
    /// </summary>
    private async Task<Result> ProcessFileAsync(CNABFile cnabFile, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Starting processing of file {FileId}: {FileName}",
            cnabFile.Id, cnabFile.FileName);

        try
        {
            // Check if physical file exists
            if (_fileStorage.FileExists(cnabFile.FilePath) is false)
            {
                var errorMsg = $"Physical file not found at {cnabFile.FilePath}";

                _logger.LogError(errorMsg);
                cnabFile.Failed(errorMsg);

                await _fileRepository.SaveChangesAsync(cancellationToken);

                return Result.Failure(errorMsg);
            }

            // Process file
            var processResult = await ProcessLinesAsync(cnabFile, cancellationToken);
            if (processResult.Succeeded is false)
            {
                cnabFile.Failed(processResult.FailureDetails.Inline());
                await _fileRepository.SaveChangesAsync(cancellationToken);

                return Result.Failure(processResult.FailureDetails);
            }

            var transactions = processResult.Value;

            // Save transactions
            if (transactions.Any())
            {
                _logger.LogInformation("Inserting {Count} transactions from file {FileId}",
                    transactions.Count, cnabFile.Id);

                //Using bulk insert for better performance
                await _transactionRepository.BulkInsertAsync(transactions, cancellationToken);
            }

            // Mark as processed
            cnabFile.Processed(transactions.Count);
            await _fileRepository.SaveChangesAsync(cancellationToken);

            _logger.LogInformation(
                "File {FileId} processed successfully. Transactions: {Count}",
                cnabFile.Id, transactions.Count);

            return Result.Success();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error processing file {FileId}: {FileName}",
                cnabFile.Id, cnabFile.FileName);

            cnabFile.Failed(ex.Message ?? ex.InnerException?.Message ?? "Error processing file");
            await _fileRepository.SaveChangesAsync(cancellationToken);

            return Result.Failure($"Processing error: {ex.Message}");
        }
    }


    private async Task<Result<List<Transaction>>> ProcessLinesAsync(
        CNABFile cnabFile,
        CancellationToken cancellationToken)
    {
        var fileStreamResult = _fileStorage.ReadFile(cnabFile.FilePath, cancellationToken);
        if (fileStreamResult.Succeeded is false)
            return Result<List<Transaction>>.Failure(fileStreamResult.FailureDetails);

        var fileStream = fileStreamResult.Value;

        using var reader = new StreamReader(fileStream);

        var transactions = new List<Transaction>();
        var errors = new List<ResultFailureDetail>();
        int lineNumber = 0;

        while (!reader.EndOfStream)
        {
            var line = await reader.ReadLineAsync(cancellationToken);
            lineNumber++;

            if (string.IsNullOrWhiteSpace(line))
                continue;

            // Parse line
            var parseResult = _parser.Parse(line);
            if (parseResult.Succeeded is false)
            {
                errors.Add(new ResultFailureDetail($"Line {lineNumber}: {parseResult.FailureDetails?.Inline()}"));
                _logger.LogWarning("Parse error on line {Line}: {Error}", lineNumber, parseResult.FailureDetails?.Inline());

                continue;
            }

            // Create Transaction
            var createResult = _factory.Create(cnabFile.Id, parseResult.Value);
            if (createResult.Succeeded is false)
            {
                errors.Add(new ResultFailureDetail($"Line {lineNumber}: {createResult.FailureDetails?.Inline()}"));
                _logger.LogWarning("Error creating transaction on line {Line}: {Error}", lineNumber, createResult.FailureDetails?.Inline());

                continue;
            }

            var transaction = createResult.Value;

            transactions.Add(transaction);
        }

        // If many errors and no valid transactions, consider failure
        if (errors.Any() && transactions.Count == 0)
        {
            return Result<List<Transaction>>.Failure(errors);
        }

        // If some errors but also valid transactions, log but continue
        if (errors.Any())
        {
            _logger.LogWarning(
                "File {FileId} processed with {ErrorCount} errors and {SuccessCount} valid transactions",
                cnabFile.Id, errors.Count, transactions.Count);
        }

        return Result<List<Transaction>>.Success(transactions);
    }
}