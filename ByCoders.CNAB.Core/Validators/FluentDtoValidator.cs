using ByCoders.CNAB.Core.Results;
using FluentValidation;

namespace ByCoders.CNAB.Core.Validators;

public abstract class FluentDtoValidator<TDto> : AbstractValidator<TDto>, IDtoValidator<TDto>
    where TDto : Dto
{

    public virtual DtoValidationResult TryValidate(TDto target)
    {
        var validationResult = Validate(target);
        var dtoValDets = validationResult.Errors
            .Select(error => new ResultFailureDetail(error.ErrorMessage, error.PropertyName))
            .ToList();

        return new DtoValidationResult(validationResult.IsValid, dtoValDets);
    }
}