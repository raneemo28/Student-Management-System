using App.Application.DTOs;
using FluentValidation;

namespace App.Application.Validators;

public class CreateAdvertisementDtoValidator : AbstractValidator<CreateAdvertisementDto>
{
    public CreateAdvertisementDtoValidator()
    {
        RuleFor(x => x.Title)
            .NotEmpty()
            .MaximumLength(200);

        RuleFor(x => x.Description)
            .MaximumLength(1000);
    }
}
