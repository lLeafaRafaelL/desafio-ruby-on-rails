namespace ByCoders.CNAB.Infrastructure.Correlation;

internal record CorrelationService : ICorrelationService
{
    public CorrelationService()
    {
        
    }

    private Guid Value { get; set; }

    public void SetCorrelationId(Guid correlationId) => Value = correlationId;
    public Guid GetCorrelationId() => Value;
}