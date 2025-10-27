using ByCoders.CNAB.Core.Validators;
using FluentValidation;

namespace ByCoders.CNAB.Application.Files.CNAB.Upload;

internal class UploadCNABFileRequestRequestValidator : FluentDtoValidator<UploadCNABFileRequest>
{
    public UploadCNABFileRequestRequestValidator()
    {
        RuleFor(x => x.File)
            .NotNull()
            .DependentRules(() =>
            {
                RuleFor(x => x.File.Length)
                    .GreaterThan(0);

                RuleFor(x => x.File.FileName)
                    .Must(x => Path.GetExtension(x).ToLowerInvariant() == ".txt")
                    .WithMessage("Invalid file type. Only .txt files are allowed");
            });
            
    }
}
