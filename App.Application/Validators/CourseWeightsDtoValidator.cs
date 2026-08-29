using App.Application.DTOs;
using FluentValidation;

namespace App.Application.Validators;

public class CourseWeightsDtoValidator : AbstractValidator<CourseWeightsDto>
{
    public CourseWeightsDtoValidator()
    {
        RuleFor(x => x.WorkWeight)
            .GreaterThanOrEqualTo(0)
            .LessThanOrEqualTo(1);

        RuleFor(x => x.FirstExamWeight)
            .GreaterThanOrEqualTo(0)
            .LessThanOrEqualTo(1);

        RuleFor(x => x.SecondExamWeight)
            .GreaterThanOrEqualTo(0)
            .LessThanOrEqualTo(1);

        RuleFor(x => x.FinalExamWeight)
            .GreaterThanOrEqualTo(0)
            .LessThanOrEqualTo(1);

        RuleFor(x => x)
            .Must(x => x.WorkWeight + x.FirstExamWeight + x.SecondExamWeight + x.FinalExamWeight == 1.0)
            .WithMessage("Weights must sum to 1.0");
    }
}
