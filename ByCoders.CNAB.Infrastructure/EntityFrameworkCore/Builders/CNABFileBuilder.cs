using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ByCoders.CNAB.Domain.Files.Models;
using Microsoft.EntityFrameworkCore;

namespace ByCoders.CNAB.Infrastructure.EntityFrameworkCore.Builders;

/// <summary>
/// EF Core configuration para entidade CNABFile
/// </summary>
public class CNABFileBuilder
{
    public static void Build(EntityTypeBuilder<CNABFile> builder)
    {
        builder.HasKey(x => x.Id);

        builder
            .Property(x => x.Id)
            .ValueGeneratedNever();

        builder.Property(x => x.FileName)
            .IsRequired()
            .HasColumnType("varchar")
            .HasMaxLength(255);

        builder.Property(x => x.FilePath)
            .IsRequired()
            .HasColumnType("varchar")
            .HasMaxLength(500);

        builder.Property(x => x.FileSize)
            .IsRequired();

        builder.Property(x => x.UploadedOn)
            .IsRequired();

        builder.Property(x => x.ProcessedOn)
            .IsRequired(false);

        builder.Property(x => x.FailedOn)
            .IsRequired(false);

        builder.Property(x => x.ErrorMessage)
            .IsRequired(false)
            .HasColumnType("varchar")
            .HasMaxLength(2000);

        builder.Property(x => x.TransactionCount)
            .IsRequired();

        builder.Ignore(x => x.Status);

        builder.HasIndex(x => x.FilePath)
            .IsUnique();

        builder.HasIndex(x => x.UploadedOn);
        
        builder.HasIndex(x => x.ProcessedOn);
        
        builder.HasIndex(x => x.FailedOn);
    }
}
