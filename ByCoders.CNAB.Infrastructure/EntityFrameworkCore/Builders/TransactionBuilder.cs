using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ByCoders.CNAB.Domain.Transactions;

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
            .HasOne(x => x.TransactionType)
            .WithOne()
            .IsRequired();

        builder
            .Property(x => x.CreatedOn)
            .IsRequired();

        builder
            .Property(x => x.TransactionDate)
            .IsRequired();

        builder
            .Property(x => x.TransactionTime)
            .IsRequired();



        builder.OwnsOne(x => x.Beneficiary, beneficiary =>
        {
            beneficiary
            .Property(x => x.Document)
            .IsRequired()
            .IsUnicode(false)
            .HasMaxLength(11);
        });

        builder.OwnsOne(x => x.Card, card =>
        {
            card
            .Property(x => x.Number)
            .IsRequired()
            .IsUnicode(false)
            .HasMaxLength(12);
        });

        builder.OwnsOne(x => x.Card, card =>
        {
            card
            .Property(x => x.Number)
            .IsRequired()
            .IsUnicode(false)
            .HasMaxLength(12);
        });

        builder.OwnsOne(x => x.Store, store =>
        {
            store
            .Property(x => x.Name)
            .IsRequired()
            .IsUnicode(false)
            .HasMaxLength(19);

            store.Property(x => x.Owner)
            .IsRequired()
            .IsUnicode(false)
            .HasMaxLength(14);

            store
            .HasIndex(x => x.Name)
            .IsUnique();
        });
    }
}