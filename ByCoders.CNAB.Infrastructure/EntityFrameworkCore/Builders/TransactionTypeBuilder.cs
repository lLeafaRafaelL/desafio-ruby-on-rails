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
            new TransactionType(TransactionTypes.Debit, "Debit", TransactionNature.CashIn),               
            new TransactionType(TransactionTypes.BankSlip, "Bank Slip", TransactionNature.CashOut),       
            new TransactionType(TransactionTypes.Funding, "Funding", TransactionNature.CashOut),          
            new TransactionType(TransactionTypes.Credit, "Credit", TransactionNature.CashIn),             
            new TransactionType(TransactionTypes.LoanReceipt, "Loan Receipt", TransactionNature.CashIn),  
            new TransactionType(TransactionTypes.Sales, "Sales", TransactionNature.CashIn),               
            new TransactionType(TransactionTypes.TEDReceipt, "TED Receipt", TransactionNature.CashIn),    
            new TransactionType(TransactionTypes.DOCReceipt, "DOC Receipt", TransactionNature.CashIn),    
            new TransactionType(TransactionTypes.Rent, "Rent", TransactionNature.CashOut)                 
        );
    }
}