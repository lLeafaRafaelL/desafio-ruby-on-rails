using ByCoders.CNAB.Core.Results;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ByCoders.CNAB.Domain.Files.Models;

/// <summary>
/// CNAB File Aggregate Root
/// Represents an uploaded CNAB file and its processing status
/// </summary>
public sealed class CNABFile
{
    private CNABFile()
    {
        Id = Guid.CreateVersion7();
        UploadedOn = DateTime.UtcNow;
    }

    public CNABFile(string fileName, string filePath, long fileSize) : this()
    {
        FileName = fileName;
        FilePath = filePath;
        FileSize = fileSize;
    }

    public Guid Id { get; init; }
    public string FileName { get; init; }
    public string FilePath { get; init; }
    public long FileSize { get; init; }
    public DateTime UploadedOn { get; private set; }
    public DateTime? ProcessingStartedOn { get; private set; }
    public DateTime? ProcessedOn { get; private set; }
    public DateTime? FailedOn { get; private set; }
    public string? ErrorMessage { get; private set; }
    public int TransactionCount { get; private set; }

    /// <summary>
    /// Gets the current status of the file (computed property)
    /// </summary>
    public CNABFileStatus Status
    {
        get
        {
            if (ProcessedOn.HasValue)
                return CNABFileStatus.Processed;

            if (FailedOn.HasValue)
                return CNABFileStatus.Failed;

            if(ProcessingStartedOn.HasValue)
                return CNABFileStatus.Processing;

            return CNABFileStatus.Uploaded;
        }
    }

    /// <summary>
    /// Marks the file as successfully processed
    /// </summary>
    public Result Processing()
    {
        if (ProcessedOn.HasValue)
            return Result.Success();

        ProcessingStartedOn = DateTime.UtcNow;

        return Result.Success();
    }

    /// <summary>
    /// Marks the file as successfully processed
    /// </summary>
    public Result Processed(int transactionCount)
    {
        if (transactionCount < 0)
            return Result.Failure("Transaction count cannot be negative", nameof(transactionCount));

        if (ProcessedOn.HasValue)
            return Result.Success();

        ProcessedOn = DateTime.UtcNow;
        TransactionCount = transactionCount;
        FailedOn = null;
        ErrorMessage = null;

        return Result.Success();
    }

    /// <summary>
    /// Marks the file as failed with an error message
    /// </summary>
    public Result Failed(string errorMessage)
    {
        if (string.IsNullOrWhiteSpace(errorMessage))
            return Result.Failure("Error message cannot be null or empty", nameof(errorMessage));

        FailedOn = DateTime.UtcNow;
        ErrorMessage = errorMessage;
        ProcessedOn = null;
        TransactionCount = 0;

        return Result.Success();
    }
}