using App.Application.DTOs;
using App.Application.Common;
using App.Application.Common.Events;
using App.Application.Repositories;
using MediatR;

namespace App.Application.CQRS.Employees.Commands;

public record UpdateEmployeeCommand(string Id, UpdateEmployeeDto Dto) : IRequest<Result<EmployeeDto>>;

public class UpdateEmployeeCommandHandler : IRequestHandler<UpdateEmployeeCommand, Result<EmployeeDto>>
{
    private readonly IUnitOfWork _unitOfWork;

    public UpdateEmployeeCommandHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<EmployeeDto>> Handle(UpdateEmployeeCommand request, CancellationToken cancellationToken)
    {
        var employee = await _unitOfWork.Employees.GetByIdAsync(request.Id);
        if (employee == null)
            return Result<EmployeeDto>.Failure("Employee not found");

        employee.FirstName = request.Dto.FirstName;
        employee.LastName = request.Dto.LastName;

        await _unitOfWork.Employees.UpdateAsync(employee);

        var domainEvent = new App.domain.entity.DomainEvent
        {
            Id = Guid.NewGuid(),
            EventType = "EmployeeUpdated",
            Data = DomainEventSerializer.Serialize(new { employee.Employee_id, employee.FirstName, employee.LastName }),
            OccurredOn = DateTime.UtcNow,
            IsProcessed = false
        };

        await _unitOfWork.DomainEvents.AddAsync(domainEvent);
        await _unitOfWork.SaveChangesAsync();

        return Result<EmployeeDto>.Success(new EmployeeDto
        {
            Employee_id = employee.Employee_id,
            FirstName = employee.FirstName,
            LastName = employee.LastName,
            User_id = employee.User_id
        });
    }
}
