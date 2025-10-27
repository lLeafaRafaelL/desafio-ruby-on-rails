using ByCoders.CNAB.Domain.Files.Models;

namespace ByCoders.CNAB.Domain.Files;

/// <summary>
/// Repository for CNABFile aggregate
/// </summary>
public interface ICNABFileRepository
{
    /// <summary>
    /// Adds a new CNAB file
    /// </summary>
    Task AddAsync(CNABFile cnabFile, CancellationToken cancellationToken);

    /// <summary>
    /// Finds file by ID
    /// </summary>
    Task<CNABFile?> GetByIdAsync(Guid id, CancellationToken cancellationToken);

    /// <summary>
    /// Finds file by path
    /// </summary>
    Task<CNABFile?> GetByFilePathAsync(string filePath, CancellationToken cancellationToken);

    /// <summary>
    /// Gets all files
    /// </summary>
    Task<IEnumerable<CNABFile>> GetAllAsync(CancellationToken cancellationToken);

    /// <summary>
    /// Finds all files by status
    /// </summary>
    Task<IEnumerable<CNABFile>> FindByStatusAsync(CNABFileStatus status, CancellationToken cancellationToken);


    /// <summary>
    /// Updates a CNAB file
    /// </summary>
    Task UpdateAsync(CNABFile cnabFile, CancellationToken cancellationToken);

    /// <summary>
    /// Persists changes to the database
    /// </summary>
    Task<int> SaveChangesAsync(CancellationToken cancellationToken);
}