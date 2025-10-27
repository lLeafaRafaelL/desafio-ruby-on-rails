using ByCoders.CNAB.Domain.Files;
using ByCoders.CNAB.Domain.Files.Models;
using ByCoders.CNAB.Infrastructure.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace ByCoders.CNAB.Infrastructure.Repositories;

/// <summary>
/// Implementação do repositório para agregado CNABFile usando Entity Framework Core
/// </summary>
public class CNABFileRepository : ICNABFileRepository
{
    private readonly CNABFileDbContext _context;

    public CNABFileRepository(CNABFileDbContext context)
    {
        _context = context;
    }

    public async Task AddAsync(CNABFile cnabFile, CancellationToken cancellationToken = default)
    {
        await _context.CNABFiles.AddAsync(cnabFile, cancellationToken);
    }

    public async Task<CNABFile?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _context.CNABFiles
            .FirstOrDefaultAsync(f => f.Id == id, cancellationToken);
    }

    public async Task<CNABFile?> GetByFilePathAsync(string filePath, CancellationToken cancellationToken = default)
    {
        return await _context.CNABFiles
            .FirstOrDefaultAsync(f => f.FilePath == filePath, cancellationToken);
    }

    public async Task<IEnumerable<CNABFile>> GetAllAsync(CNABFileStatus? status = null, CancellationToken cancellationToken = default)
    {
        var query = _context.CNABFiles.AsQueryable();

        if (status.HasValue)
        {
            // Filtrar por status usando computed property
            query = status.Value switch
            {
                CNABFileStatus.Uploaded => query.Where(f => f.ProcessedOn == null && f.FailedOn == null),
                CNABFileStatus.Processed => query.Where(f => f.ProcessedOn != null),
                CNABFileStatus.Failed => query.Where(f => f.FailedOn != null),
                _ => query
            };
        }

        return await query
            .OrderByDescending(f => f.UploadedOn)
            .ToListAsync(cancellationToken);
    }

    public async Task UpdateAsync(CNABFile cnabFile, CancellationToken cancellationToken = default)
    {
        _context.CNABFiles.Update(cnabFile);
        await Task.CompletedTask;
    }

    public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        return await _context.SaveChangesAsync(cancellationToken);
    }

    public Task<IEnumerable<CNABFile>> GetAllAsync(CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    public Task<IEnumerable<CNABFile>> FindByStatusAsync(CNABFileStatus status, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}
