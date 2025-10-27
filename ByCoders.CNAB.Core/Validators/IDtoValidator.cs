namespace ByCoders.CNAB.Core.Validators;

public interface IDtoValidator<in TDto> where TDto : Dto
{
    DtoValidationResult TryValidate(TDto target);
}
