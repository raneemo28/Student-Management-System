using App.Application.DTOs;
using FluentValidation;

namespace App.Application.Validators;

public class UpdateEmployeeDtoValidator : AbstractValidator<UpdateEmployeeDto>
{
    public UpdateEmployeeDtoValidator()
    {
        RuleFor(x => x.FirstName)
            .NotEmpty()
            .MaximumLength(100);

        RuleFor(x => x.LastName)
            .NotEmpty()
            .MaximumLength(100);
    }
}
