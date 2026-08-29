using App.Application.DTOs;
using FluentValidation;

namespace App.Application.Validators;

public class CreateCourseDtoValidator : AbstractValidator<CreateCourseDto>
{
    public CreateCourseDtoValidator()
    {
        RuleFor(x => x.Course_name)
            .NotEmpty()
            .MaximumLength(200);
    }
}
