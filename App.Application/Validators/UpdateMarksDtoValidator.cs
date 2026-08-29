using App.Application.DTOs;
using FluentValidation;

namespace App.Application.Validators;

public class UpdateMarksDtoValidator : AbstractValidator<UpdateMarksDto>
{
    public UpdateMarksDtoValidator()
    {
        RuleFor(x => x.WorkMark)
            .GreaterThanOrEqualTo(0)
            .LessThanOrEqualTo(100)
            .When(x => x.WorkMark.HasValue);

        RuleFor(x => x.FirstExamMark)
            .GreaterThanOrEqualTo(0)
            .LessThanOrEqualTo(100)
            .When(x => x.FirstExamMark.HasValue);

        RuleFor(x => x.SecondExamMark)
            .GreaterThanOrEqualTo(0)
            .LessThanOrEqualTo(100)
            .When(x => x.SecondExamMark.HasValue);
    }
}
