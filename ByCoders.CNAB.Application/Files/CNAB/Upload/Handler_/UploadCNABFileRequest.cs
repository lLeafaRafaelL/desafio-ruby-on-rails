using ByCoders.CNAB.Core;
using Microsoft.AspNetCore.Http;

namespace ByCoders.CNAB.Application.Files.CNAB.Upload;

/// <summary>
/// Request para upload de arquivo CNAB
/// </summary>
public record UploadCNABFileRequest(IFormFile File) : Dto;
