using App.Application.DTOs;
using FluentValidation;

namespace App.Application.Validators;

public class CreateHomeworkDtoValidator : AbstractValidator<CreateHomeworkDto>
{
    public CreateHomeworkDtoValidator()
    {
        RuleFor(x => x.Course_id)
            .NotEmpty();

        RuleFor(x => x.Title)
            .NotEmpty()
            .MaximumLength(200);

        RuleFor(x => x.Description)
            .MaximumLength(2000);

        RuleFor(x => x.DueDate)
            .GreaterThan(DateTime.UtcNow)
            .WithMessage("Due date must be in the future");

        RuleFor(x => x.TotalMarks)
            .GreaterThan(0);

        RuleFor(x => x.QuestionFileName)
            .NotEmpty();

        RuleFor(x => x.QuestionFileBytes)
            .Must(f => f != null && f.Length > 0)
            .WithMessage("Question file is required");
    }
}
