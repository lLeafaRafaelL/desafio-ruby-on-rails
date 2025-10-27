namespace ByCoders.CNAB.Infrastructure.Correlation;

internal record CorrelationId : ICorrelation
{
    public CorrelationId()
    {
        
    }

    private Guid Value { get; set; }

    public void SetCorrelationId(Guid correlationId) => Value = correlationId;
    public Guid GetCorrelationId() => Value;
}