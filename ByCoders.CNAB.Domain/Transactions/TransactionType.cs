namespace ByCoders.CNAB.Domain.Transactions;

public record TransactionType
{
    private TransactionType()
    {
        
    }

    public TransactionType(TransactionTypes transactionType)
    {
        Id = (int) transactionType;
    }

    public TransactionType(TransactionTypes transactionType, string description, TransactionNature nature)
        : this(transactionType)
    {
        Description = description;
        Nature = nature;
    }

    public int Id { get; set; }
    public string Description { get; set; }
    public TransactionNature Nature { get; set; }
}