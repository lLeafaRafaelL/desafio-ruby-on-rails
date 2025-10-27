using ByCoders.CNAB.Core.Results;

namespace ByCoders.CNAB.Domain.Files;

/// <summary>
/// File storage service
/// Storage abstraction (file system, S3, blob, etc)
/// </summary>
public interface IFileStorageService
{

    Task<Result<string>> SaveFileAsync(string fileName, Stream fileStream, CancellationToken cancellationToken);
    Result<Stream> ReadFile(string filePath, CancellationToken cancellationToken);
    bool FileExists(string filePath);
}