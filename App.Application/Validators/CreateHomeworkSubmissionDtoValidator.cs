using App.Application.DTOs;
using FluentValidation;

namespace App.Application.Validators;

public class CreateHomeworkSubmissionDtoValidator : AbstractValidator<CreateHomeworkSubmissionDto>
{
    public CreateHomeworkSubmissionDtoValidator()
    {
        RuleFor(x => x.Homework_id)
            .NotEmpty();

        RuleFor(x => x.Student_id)
            .NotEmpty();

        RuleFor(x => x.SolutionFileName)
            .NotEmpty();

        RuleFor(x => x.SolutionFileBytes)
            .Must(f => f != null && f.Length > 0)
            .WithMessage("Solution file is required");
    }
}
