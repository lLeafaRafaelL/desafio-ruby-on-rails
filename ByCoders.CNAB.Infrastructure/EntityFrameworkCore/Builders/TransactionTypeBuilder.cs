using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ByCoders.CNAB.Domain.Transactions.Models;

namespace ByCoders.CNAB.Infrastructure.EntityFrameworkCore.Builders;

public class TransactionTypeBuilder
{
    public static void Build(EntityTypeBuilder<TransactionType> builder)
    {
        builder.HasKey(x => x.Id);

        builder
            .Property(x => x.Id)
            .ValueGeneratedNever();

        builder.Property(x => x.Description)
            .IsUnicode(false)
            .HasMaxLength(30)
            .IsRequired();

        builder.Property(x => x.Nature)
            .IsRequired()
            .HasConversion<byte>();

        builder.HasData(
            new TransactionType(TransactionTypes.Debit, "Débito", TransactionNature.CashOut),
            new TransactionType(TransactionTypes.BankSlip, "Débito", TransactionNature.CashOut),
            new TransactionType(TransactionTypes.Funding, "Débito", TransactionNature.CashOut),
            new TransactionType(TransactionTypes.Credit, "Débito", TransactionNature.CashIn),
            new TransactionType(TransactionTypes.LoanReceipt, "Débito", TransactionNature.CashIn),
            new TransactionType(TransactionTypes.Sales, "Débito", TransactionNature.CashIn),
            new TransactionType(TransactionTypes.TEDReceipt, "Débito", TransactionNature.CashIn),
            new TransactionType(TransactionTypes.DOCReceipt, "Débito", TransactionNature.CashIn),
            new TransactionType(TransactionTypes.Rent, "Débito", TransactionNature.CashOut)
            );
    }
}