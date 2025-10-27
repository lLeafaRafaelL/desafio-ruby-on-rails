using ByCoders.CNAB.Domain.Files;
using ByCoders.CNAB.Domain.Files.Models;
using ByCoders.CNAB.Infrastructure.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace ByCoders.CNAB.Infrastructure.Repositories;

/// <summary>
/// Implementation of the repository for the CNABFile aggregate using Entity Framework Core
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

    public async Task<IEnumerable<CNABFile>> FindByStatusAsync(CNABFileStatus status, CancellationToken cancellationToken)
    {
        var query = _context.CNABFiles.AsQueryable();

            // Filter by status using computed property
            query = status switch
            {
                CNABFileStatus.Uploaded => query.Where(f => f.ProcessedOn == null && f.FailedOn == null),
                CNABFileStatus.Processed => query.Where(f => f.ProcessedOn != null),
                CNABFileStatus.Failed => query.Where(f => f.FailedOn != null),
                _ => query
            };
        
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
}
