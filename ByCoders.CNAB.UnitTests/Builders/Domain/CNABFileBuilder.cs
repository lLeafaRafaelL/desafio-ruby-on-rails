using ByCoders.CNAB.Domain.Files.Models;
using FizzWare.NBuilder;
using System.Linq;

namespace ByCoders.CNAB.UnitTests.Builders.Domain;

public class CNABFileBuilder
{
    private string _fileName = "CNAB.txt";
    private string _filePath = "/storage/cnab-files/20241024_123456_abc123_CNAB.txt";
    private long _fileSize = 1024L;
    private bool _markAsProcessed = false;
    private int _transactionCount = 0;
    private bool _markAsFailed = false;
    private string? _errorMessage = null;

    public static CNABFileBuilder New => new();

    public CNABFileBuilder WithFileName(string fileName)
    {
        _fileName = fileName;
        return this;
    }

    public CNABFileBuilder WithFilePath(string filePath)
    {
        _filePath = filePath;
        return this;
    }

    public CNABFileBuilder WithFileSize(long fileSize)
    {
        _fileSize = fileSize;
        return this;
    }

    public CNABFileBuilder WithSmallFile()
    {
        _fileSize = 100L;
        return this;
    }

    public CNABFileBuilder WithLargeFile()
    {
        _fileSize = 1024 * 1024 * 10L; // 10MB
        return this;
    }

    public CNABFileBuilder AsProcessed(int transactionCount = 21)
    {
        _markAsProcessed = true;
        _transactionCount = transactionCount;
        return this;
    }

    public CNABFileBuilder AsFailed(string errorMessage = "Parse error on line 5")
    {
        _markAsFailed = true;
        _errorMessage = errorMessage;
        return this;
    }

    public CNABFileBuilder WithRandomData()
    {
        var random = new Random();
        var timestamp = DateTime.UtcNow.ToString("yyyyMMdd_HHmmss");
        var uniqueId = Guid.NewGuid().ToString("N").Substring(0, 6);
        
        _fileName = $"CNAB_{timestamp}.txt";
        _filePath = $"/storage/cnab-files/{timestamp}_{uniqueId}_CNAB.txt";
        _fileSize = random.Next(100, 100000);
        
        return this;
    }

    public CNABFile Build()
    {
        var cnabFile = new CNABFile(_fileName, _filePath, _fileSize);
        
        if (_markAsProcessed)
        {
            var result = cnabFile.Processed(_transactionCount);
            if (!result.Succeeded)
                throw new InvalidOperationException($"Failed to mark file as processed: {string.Join(", ", result.FailureDetails.Select(x => x.Description))}");
        }
        else if (_markAsFailed)
        {
            var result = cnabFile.Failed(_errorMessage!);
            if (!result.Succeeded)
                throw new InvalidOperationException($"Failed to mark file as failed: {string.Join(", ", result.FailureDetails.Select(x => x.Description))}");
        }
        
        return cnabFile;
    }

    public static implicit operator CNABFile(CNABFileBuilder builder)
    {
        return builder.Build();
    }
}
