using App.Application.DTOs;
using FluentValidation;

namespace App.Application.Validators;

public class CreateHomeworkQuestionMarkDtoValidator : AbstractValidator<CreateHomeworkQuestionMarkDto>
{
    public CreateHomeworkQuestionMarkDtoValidator()
    {
        RuleFor(x => x.Submission_id)
            .NotEmpty();

        RuleFor(x => x.QuestionNumber)
            .GreaterThan(0);

        RuleFor(x => x.QuestionDescription)
            .NotEmpty()
            .MaximumLength(500);

        RuleFor(x => x.MaxMarks)
            .GreaterThan(0);

        RuleFor(x => x.ObtainedMarks)
            .GreaterThanOrEqualTo(0)
            .LessThanOrEqualTo(x => x.MaxMarks)
            .When(x => x.ObtainedMarks.HasValue);
    }
}
