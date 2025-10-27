using ByCoders.CNAB.Core.Validators;
using FluentValidation;

namespace ByCoders.CNAB.Application.Files.CNAB.Upload;

internal class UploadCNABFileRequestRequestValidator : FluentDtoValidator<UploadCNABFileRequest>
{
    public UploadCNABFileRequestRequestValidator()
    {
        RuleFor(x => x.File)
            .NotNull()
            .Must(x => x.Length > 0)
            .Must(x => Path.GetExtension(x.FileName).ToLowerInvariant() == ".txt")
            .WithMessage("Invalid file type. Only .txt files are allowed");
    }
}
