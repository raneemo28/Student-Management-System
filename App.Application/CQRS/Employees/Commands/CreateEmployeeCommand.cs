using App.Application.DTOs;
using App.Application.Common;
using App.Application.Common.Events;
using App.Application.Repositories;
using MediatR;

namespace App.Application.CQRS.Employees.Commands;

public record CreateEmployeeCommand(CreateEmployeeDto Dto) : IRequest<Result<EmployeeDto>>;

public class CreateEmployeeCommandHandler : IRequestHandler<CreateEmployeeCommand, Result<EmployeeDto>>
{
    private readonly IUnitOfWork _unitOfWork;

    public CreateEmployeeCommandHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<EmployeeDto>> Handle(CreateEmployeeCommand request, CancellationToken cancellationToken)
    {
        var employee = new App.domain.entity.Employee
        {
            FirstName = request.Dto.FirstName,
            LastName = request.Dto.LastName,
            Employee_id = Guid.NewGuid().ToString()
        };

        await _unitOfWork.Employees.AddAsync(employee);

        var domainEvent = new App.domain.entity.DomainEvent
        {
            Id = Guid.NewGuid(),
            EventType = "EmployeeCreated",
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
