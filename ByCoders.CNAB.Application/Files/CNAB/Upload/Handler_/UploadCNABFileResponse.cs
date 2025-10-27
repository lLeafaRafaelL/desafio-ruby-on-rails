using ByCoders.CNAB.Core;
using ByCoders.CNAB.Domain.Files.Models;
using FluentValidation.Results;

namespace ByCoders.CNAB.Application.Files.CNAB.Upload;

public record UploadCNABFileResponse : Dto
{
    public Guid FileId { get; set; }
    public string? FileName { get; set; }
    public CNABFileStatus Status { get; set; }
}