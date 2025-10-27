using ByCoders.CNAB.API.Configurations;
using ByCoders.CNAB.Core.Results;
using ByCoders.CNAB.Domain.Files;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace ByCoders.CNAB.Infrastructure.Storage;

public class FileStorageService : IFileStorageService
{
    private readonly FileStorageConfiguration _fileStorageConfiguration;
    private readonly ILogger<FileStorageService> _logger;

    public FileStorageService(IOptions<FileStorageConfiguration> fileStorageConfiguration, ILogger<FileStorageService> logger)
    {
        _fileStorageConfiguration = fileStorageConfiguration.Value;
        _logger = logger;
    }

    public async Task<Result<string>> SaveFileAsync(string fileName, Stream fileStream, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(fileName))
            return Result<string>.Failure("Invalid file name.");

        if (fileStream == null)
            return Result<string>.Failure("Invalid file stream.");;

        var timestamp = DateTime.UtcNow.ToString("yyyyMMdd_HHmmss");
        var uniqueFileName = $"{timestamp}_{Guid.NewGuid():N}_{Path.GetFileName(fileName)}";
        var relativePath = Path.Combine("cnab-files", uniqueFileName);

        var fullPath = Path.Combine(_fileStorageConfiguration.StoragePath, relativePath);
        var directory = Path.GetDirectoryName(fullPath);

        if (!string.IsNullOrEmpty(directory) && !Directory.Exists(directory))
        {
            Directory.CreateDirectory(directory);
        }

        try
        {
            await using var fileStreamOut = new FileStream(fullPath, FileMode.Create, FileAccess.Write, FileShare.None, 4096, true);
            await fileStream.CopyToAsync(fileStreamOut, cancellationToken);

            _logger.LogInformation("File Saved: {FileName} -> {Path}", fileName, relativePath);
            
             var filePath = relativePath.Replace("\\", "/");

            return Result<string>.Success(filePath);

        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error saving file {FileName}", fileName);
            return Result<string>.Failure($"Error saving file: {ex.Message}");
        }
    }

    public Result<Stream> ReadFile(string filePath, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(filePath))
            return Result<Stream>.Failure("Invalid file path.");

        var fullPath = Path.Combine(_fileStorageConfiguration.StoragePath, filePath);

        if (FileExists(filePath) is false)
        {
            _logger.LogWarning("File not found: {Path}", filePath);
            return Result<Stream>.Failure($"File not found: {filePath}");
        }

        try
        {
            //Return file only for read
            var fileStream = new FileStream(fullPath, FileMode.Open, FileAccess.Read, FileShare.Read, 4096, true);
            _logger.LogDebug("File opened for reading: {Path}", filePath);

            return Result<Stream>.Success(fileStream);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error reading file {Path}", filePath);
            return Result<Stream>.Failure($"Error reading file: {ex.Message}");
        }
    }

    public bool FileExists(string filePath)
    {
        if (string.IsNullOrWhiteSpace(filePath))
            return false;

        var fullPath = Path.Combine(_fileStorageConfiguration.StoragePath, filePath);

        return File.Exists(fullPath);
    }
}
