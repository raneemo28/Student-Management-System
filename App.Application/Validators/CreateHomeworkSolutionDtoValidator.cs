using App.Application.DTOs;
using FluentValidation;

namespace App.Application.Validators;

public class CreateHomeworkSolutionDtoValidator : AbstractValidator<CreateHomeworkSolutionDto>
{
    public CreateHomeworkSolutionDtoValidator()
    {
        RuleFor(x => x.Homework_id)
            .NotEmpty();

        RuleFor(x => x.SolutionFileName)
            .NotEmpty();

        RuleFor(x => x.SolutionFileBytes)
            .Must(f => f != null && f.Length > 0)
            .WithMessage("Solution file is required");
    }
}
