using ByCoders.CNAB.Application.Files.CNAB.Upload;
using ByCoders.CNAB.Core;
using ByCoders.CNAB.Core.Http;
using ByCoders.CNAB.Core.Results;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Rewrite;

namespace ByCoders.CNAB.API.Controllers.Files;

/// <summary>
/// Controller to manage CNAB Files
/// Aggregate Root: CNABFile
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public class FilesController : BaseController
{
    private readonly IRequestHandler<UploadCNABFileRequest, UploadCNABFileResponse> _handler;

    public FilesController(
        IRequestHandler<UploadCNABFileRequest, UploadCNABFileResponse> handler,
        ILogger<FilesController> logger) : base(logger)
    {
        _handler = handler;
    }

    /// <summary>
    /// Upload CNAB File
    /// </summary>
    /// <param name="file">CNAB File (.txt)</param>
    /// <param name="cancellationToken">Cancellation Token</param>
    /// <returns>CNAB uploaded file information</returns>
    /// <response code="202">File uploaded successfully and awaiting processing</response>
    /// <response code="400">Invalid file or processing error</response>
    [HttpPost]
    [ProducesResponseType(typeof(UploadCNABFileResponse), StatusCodes.Status202Accepted)]
    [ProducesResponseType(typeof(ResultFailureDetail), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> UploadAsync(
        [FromForm] IFormFile file,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation("CNAB File received: {FileName}", file?.FileName);

        var request = new UploadCNABFileRequest(file);

        var result = await _handler.HandleAsync(request, cancellationToken);

        return ResponseToActionResult(result, _ => result.Value, _ => result.FailureDetails);
    }
}