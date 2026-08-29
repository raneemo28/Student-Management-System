using App.Application.DTOs;
using FluentValidation;

namespace App.Application.Validators;

public class UpdateHomeworkDtoValidator : AbstractValidator<UpdateHomeworkDto>
{
    public UpdateHomeworkDtoValidator()
    {
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

        When(x => x.QuestionFileBytes != null && x.QuestionFileBytes.Length > 0, () =>
        {
            RuleFor(x => x.QuestionFileName).NotEmpty();
        });
    }
}
