namespace ByCoders.CNAB.Domain.Files.Models;

/// <summary>
/// 0 - Uploaded
/// 1 - Processing
/// 2 - Processed
/// 3 - Failed
/// </summary>
public enum CNABFileStatus
{
    /// <summary>
    /// 0 - Uploaded
    /// </summary>
    Uploaded,
    /// <summary>
    /// 1 - Processing
    /// </summary>
    Processing,
    /// <summary>
    /// 2 Processed
    /// </summary>
    Processed,
    /// <summary>
    /// 3 - Failed
    /// </summary>
    Failed
}