using Microsoft.EntityFrameworkCore;
using ByCoders.CNAB.Infrastructure.EntityFrameworkCore.Builders;
using ByCoders.CNAB.Domain.Files.Models;

namespace ByCoders.CNAB.Infrastructure.EntityFrameworkCore.Configurations;

/// <summary>
/// DbContext dedicado para o agregado CNABFile
/// Bounded Context: Gestão de Arquivos CNAB
/// </summary>
public class CNABFileDbContext : DbContext
{
    public CNABFileDbContext(DbContextOptions<CNABFileDbContext> options) : base(options) { }

    public DbSet<CNABFile> CNABFiles { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        CNABFileBuilder.Build(modelBuilder.Entity<CNABFile>());
    }
}
