using App.Application.DTOs;
using FluentValidation;

namespace App.Application.Validators;

public class UpdateCourseDtoValidator : AbstractValidator<UpdateCourseDto>
{
    public UpdateCourseDtoValidator()
    {
        RuleFor(x => x.Course_name)
            .NotEmpty()
            .MaximumLength(200);
    }
}
