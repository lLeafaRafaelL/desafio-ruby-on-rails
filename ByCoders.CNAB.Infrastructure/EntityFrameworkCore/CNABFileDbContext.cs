using ByCoders.CNAB.Domain.Files.Models;
using ByCoders.CNAB.Domain.Transactions.Models;
using ByCoders.CNAB.Infrastructure.EntityFrameworkCore.Builders;
using Microsoft.EntityFrameworkCore;

namespace ByCoders.CNAB.Infrastructure.EntityFrameworkCore;

/// <summary>
/// DbContext dedicado para o agregado CNABFile
/// Bounded Context: Gestão de Arquivos CNAB
/// </summary>
public class CNABFileDbContext : DbContext
{
    public CNABFileDbContext(DbContextOptions<CNABFileDbContext> options) : base(options) { }

    public DbSet<CNABFile> CNABFiles { get; set; }

    public DbSet<Transaction> Transactions { get; set; }
    public DbSet<TransactionType> TransactionTypes { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        CNABFileBuilder.Build(modelBuilder.Entity<CNABFile>());
        TransactionBuilder.Build(modelBuilder.Entity<Transaction>());
        TransactionTypeBuilder.Build(modelBuilder.Entity<TransactionType>());
    }
}