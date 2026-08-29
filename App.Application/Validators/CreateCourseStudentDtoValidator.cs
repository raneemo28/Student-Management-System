using App.Application.DTOs;
using FluentValidation;

namespace App.Application.Validators;

public class CreateCourseStudentDtoValidator : AbstractValidator<CreateCourseStudentDto>
{
    public CreateCourseStudentDtoValidator()
    {
        RuleFor(x => x.Student_id)
            .NotEmpty();

        RuleFor(x => x.Course_id)
            .NotEmpty();
    }
}
