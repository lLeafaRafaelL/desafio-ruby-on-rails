using Microsoft.Extensions.DependencyInjection;

namespace ByCoders.CNAB.Core.BackgroundServices;

internal sealed class ScopedService : IScoopedService
{
    private readonly IServiceProvider _provider;
    private readonly Func<IServiceProvider, IScoopedService> _targets;
    public ScopedService(IServiceProvider provider, Func<IServiceProvider, IScoopedService> target)
    {
        _provider = provider;
        _targets = target;
    }
    public async Task ExecuteAsync(CancellationToken ct)
    {
        using var scope = _provider.CreateScope();
        var targetDispatcher = _targets(scope.ServiceProvider);

        await targetDispatcher.ExecuteAsync(ct);
    }
    public void Dispose() { }
}
