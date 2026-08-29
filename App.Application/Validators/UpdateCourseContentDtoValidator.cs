using App.Application.DTOs;
using FluentValidation;

namespace App.Application.Validators;

public class UpdateCourseContentDtoValidator : AbstractValidator<UpdateCourseContentDto>
{
    public UpdateCourseContentDtoValidator()
    {
        RuleFor(x => x.Title)
            .NotEmpty()
            .MaximumLength(200);

        RuleFor(x => x.Description)
            .MaximumLength(1000);

        RuleFor(x => x.ContentType)
            .NotEmpty()
            .Must(x => x == "Pdf" || x == "Image" || x == "Video")
            .WithMessage("ContentType must be Pdf, Image, or Video");

        When(x => x.FileBytes != null && x.FileBytes.Length > 0, () =>
        {
            RuleFor(x => x.FileName).NotEmpty();
            RuleFor(x => x.FileSize).GreaterThan(0);
        });
    }
}
