using ByCoders.CNAB.Application.Files.CNAB.Process;
using ByCoders.CNAB.Core.BackgroundServices;
using ByCoders.CNAB.Worker.Configurations;
using Microsoft.Extensions.Options;

namespace ByCoders.CNAB.Worker.Files;

/// <summary>
/// CNAB file processing service
/// Responsible for processing uploaded files and creating transactions
/// </summary>
/// 
public class CNABFileProcessor : IScoopedService
{
    private readonly TimeSpan _pollingInterval;
    private readonly IProcessCNABFileService _processorService;
    private readonly ILogger<CNABFileProcessor> _logger;

    public CNABFileProcessor(
        IOptions<CNABFileProcessorConfiguration> configuration,
        IProcessCNABFileService processService,
        ILogger<CNABFileProcessor> logger)
    {
        _pollingInterval = TimeSpan.FromSeconds(configuration.Value.PoolingIntervalSeconds);
        _processorService = processService ?? throw new ArgumentNullException(nameof(processService));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }


    public async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        await Task.Delay(_pollingInterval, stoppingToken);

        _logger.LogInformation("CNAB File Processor Service started at: {Time}", DateTimeOffset.Now);

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                var processedCount = await _processorService.ProcessPendingFilesAsync(stoppingToken);
                if (processedCount > 0)
                {
                    _logger.LogInformation("Processed {Count} CNAB file(s) in this execution", processedCount);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing CNAB files");
            }

            _logger.LogDebug("Waiting {Interval} seconds before next poll", _pollingInterval.TotalSeconds);

            await Task.Delay(_pollingInterval, stoppingToken);
        }

        _logger.LogInformation("CNAB File Processor Service stopped at: {Time}", DateTimeOffset.Now);
    }

}