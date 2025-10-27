namespace ByCoders.CNAB.Application.Files.CNAB.Process;

/// <summary>
/// CNAB file processing service
/// Responsible for processing uploaded files and creating transactions
/// </summary>
/// 


public interface IProcessCNABFileService
{
    Task<int> ProcessPendingFilesAsync(CancellationToken cancellationToken);
}
