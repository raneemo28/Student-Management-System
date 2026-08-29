using App.Application.DTOs;
using FluentValidation;

namespace App.Application.Validators;

public class UpdateAdvertisementDtoValidator : AbstractValidator<UpdateAdvertisementDto>
{
    public UpdateAdvertisementDtoValidator()
    {
        RuleFor(x => x.Title)
            .NotEmpty()
            .MaximumLength(200);

        RuleFor(x => x.Description)
            .MaximumLength(1000);
    }
}
