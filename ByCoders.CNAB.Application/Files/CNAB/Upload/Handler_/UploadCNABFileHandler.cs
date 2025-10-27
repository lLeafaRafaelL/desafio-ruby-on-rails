using ByCoders.CNAB.Core;
using ByCoders.CNAB.Core.Validators;
using ByCoders.CNAB.Domain.Files;
using ByCoders.CNAB.Domain.Files.Models;
using Microsoft.Extensions.Logging;

namespace ByCoders.CNAB.Application.Files.CNAB.Upload;

public class UploadCNABFileHandler : RequestHandler<UploadCNABFileRequest, UploadCNABFileResponse>
{
    private readonly IFileStorageService _fileStorage;
    private readonly ICNABFileRepository _fileRepository;
    private readonly ILogger<UploadCNABFileHandler> _logger;
    private readonly IDtoValidator<UploadCNABFileRequest> _validator;

    public UploadCNABFileHandler(
        IFileStorageService fileStorage,
        ICNABFileRepository fileRepository,
        ILogger<UploadCNABFileHandler> logger,
        IDtoValidator<UploadCNABFileRequest> validator)
    {
        _fileStorage = fileStorage ?? throw new ArgumentNullException(nameof(fileStorage));
        _fileRepository = fileRepository ?? throw new ArgumentNullException(nameof(fileRepository));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _validator = validator ?? throw new ArgumentNullException(nameof(validator));
    }

    public override async Task<RequestHandlerResult<UploadCNABFileResponse>> HandleAsync(UploadCNABFileRequest request, CancellationToken cancellationToken)
    {
        var validationResult = _validator.TryValidate(request);
        if (validationResult.IsValid is false)
            return RequestHandlerResult<UploadCNABFileResponse>.Unprocessable(failureDetails: validationResult.FailureDetails);

        _logger.LogInformation("Uploading file: {FileName} ({Size} bytes)", request.File.FileName, request.File.Length);

        await using var stream = request.File.OpenReadStream();
        var fileResult = await _fileStorage.SaveFileAsync(
            request.File.FileName,
            stream,
            cancellationToken);

        if(fileResult.Succeeded is false)
            return RequestHandlerResult<UploadCNABFileResponse>.Unprocessable(failureDetails: fileResult.FailureDetails);

        var cnabFile = new CNABFile(
            request.File.FileName,
            fileResult.Value,
            request.File.Length);

        await _fileRepository.AddAsync(cnabFile, cancellationToken);
        await _fileRepository.SaveChangesAsync(cancellationToken);

        _logger.LogInformation(
            "File uploaded successfully. FileId: {FileId}, Path: {Path}",
            cnabFile.Id, fileResult.Value);

        return RequestHandlerResult<UploadCNABFileResponse>.Accepted(
            new UploadCNABFileResponse
            {
                FileId = cnabFile.Id,
                FileName = cnabFile.FileName,
                Status = cnabFile.Status,
            });
    }
}