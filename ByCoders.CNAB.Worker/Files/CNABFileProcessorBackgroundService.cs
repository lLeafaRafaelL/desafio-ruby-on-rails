using ByCoders.CNAB.Core.BackgroundServices;
using ByCoders.CNAB.Infrastructure.Repositories;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Serilog;

namespace ByCoders.CNAB.Worker.Files;

/// <summary>
/// Background Service: Processes pending CNAB files
/// Runs continuously searching for files with Status = Uploaded
/// </summary>
public class CNABFileProcessorBackgroundService : BackgroundService
{
    private readonly IScoopedService _target;

    public CNABFileProcessorBackgroundService(
        IScoopedService provider)
    {
        _target = provider;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        await Task.Run(async () => await _target.ExecuteAsync(stoppingToken), stoppingToken);
    }
}