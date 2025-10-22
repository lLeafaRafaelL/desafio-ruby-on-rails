using Microsoft.EntityFrameworkCore;
using ByCoders.CNAB.Domain.Transactions;
using ByCoders.CNAB.Infrastructure.EntityFrameworkCore.Builders;

namespace ByCoders.CNAB.Infrastructure.EntityFrameworkCore.Configurations;

public class TransactionDbContext : DbContext
{
    public TransactionDbContext(DbContextOptions<TransactionDbContext> options) : base(options) { }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {

        base.OnModelCreating(modelBuilder);

        TransactionBuilder.Build(modelBuilder.Entity<Transaction>());
        TransactionTypeBuilder.Build(modelBuilder.Entity<TransactionType>());
    }
}