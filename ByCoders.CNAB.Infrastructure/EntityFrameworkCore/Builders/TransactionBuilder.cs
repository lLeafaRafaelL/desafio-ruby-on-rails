using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ByCoders.CNAB.Domain.Transactions.Models;
using Microsoft.EntityFrameworkCore;

namespace ByCoders.CNAB.Infrastructure.EntityFrameworkCore.Builders;

public class TransactionBuilder
{
    public static void Build(EntityTypeBuilder<Transaction> builder)
    {
        builder.HasDiscriminator<TransactionTypes>("TransactionTypeId")
           .HasValue<Debit>(TransactionTypes.Debit)
           .HasValue<BankSlip>(TransactionTypes.BankSlip)
           .HasValue<Funding>(TransactionTypes.Funding)
           .HasValue<Credit>(TransactionTypes.Credit)
           .HasValue<LoanReceipt>(TransactionTypes.LoanReceipt)
           .HasValue<Sale>(TransactionTypes.Sales)
           .HasValue<TEDReceipt>(TransactionTypes.TEDReceipt)
           .HasValue<DOCReceipt>(TransactionTypes.DOCReceipt)
           .HasValue<Rent>(TransactionTypes.Rent);

        builder.HasKey(x => x.Id);

        builder
            .Property(x => x.Id)
            .ValueGeneratedNever();

        builder
            .HasOne(x => x.TransactionType);

        builder
            .Property(x => x.CreatedOn)
            .IsRequired();

        builder
            .Property(x => x.TransactionDateTime)
            .IsRequired();

        builder
            .HasIndex(x => x.TransactionDateTime)
            .IsDescending();

        builder
            .Ignore(x => x.TransactionDate); 
        
        builder
            .Ignore(x => x.TransactionTime);

        builder
            .Property(x => x.CNABFileId)
            .IsRequired(false);

        builder
            .HasIndex(x => x.CNABFileId);

        builder.OwnsOne(x => x.Beneficiary, beneficiary =>
        {
            beneficiary
            .Property(x => x.Document)
            .IsRequired()
            .HasColumnType("varchar")
            .HasMaxLength(11);
        });

        builder.OwnsOne(x => x.Card, card =>
        {
            card
            .Property(x => x.Number)
            .IsRequired()
            .HasColumnType("varchar")
            .HasMaxLength(12);
        });

        builder.OwnsOne(x => x.Store, store =>
        {
            store
            .Property(x => x.Name)
            .IsRequired()
            .HasColumnType("varchar")
            .HasMaxLength(19);

            store.Property(x => x.Owner)
            .IsRequired()
            .HasColumnType("varchar")
            .HasMaxLength(14);

            store
            .HasIndex(x => x.Name)
            .IsUnique();
        });
    }
}