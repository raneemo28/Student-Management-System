using App.Application.DTOs;
using FluentValidation;

namespace App.Application.Validators;

public class CreateCourseContentDtoValidator : AbstractValidator<CreateCourseContentDto>
{
    public CreateCourseContentDtoValidator()
    {
        RuleFor(x => x.Course_id)
            .NotEmpty();

        RuleFor(x => x.Title)
            .NotEmpty()
            .MaximumLength(200);

        RuleFor(x => x.Description)
            .MaximumLength(1000);

        RuleFor(x => x.ContentType)
            .NotEmpty()
            .Must(x => x == "Pdf" || x == "Image" || x == "Video")
            .WithMessage("ContentType must be Pdf, Image, or Video");

        RuleFor(x => x.FileName)
            .NotEmpty();

        RuleFor(x => x.FileBytes)
            .Must(f => f != null && f.Length > 0)
            .WithMessage("File is required");
    }
}
