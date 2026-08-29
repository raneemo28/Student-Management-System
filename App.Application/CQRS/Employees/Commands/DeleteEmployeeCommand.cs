using App.Application.Common;
using App.Application.Common.Events;
using App.Application.Repositories;
using MediatR;

namespace App.Application.CQRS.Employees.Commands;

public record DeleteEmployeeCommand(string Id) : IRequest<Result>;

public class DeleteEmployeeCommandHandler : IRequestHandler<DeleteEmployeeCommand, Result>
{
    private readonly IUnitOfWork _unitOfWork;

    public DeleteEmployeeCommandHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<Result> Handle(DeleteEmployeeCommand request, CancellationToken cancellationToken)
    {
        var employee = await _unitOfWork.Employees.GetByIdAsync(request.Id);
        if (employee == null)
            return Result.Failure("Employee not found");

        await _unitOfWork.Employees.DeleteAsync(employee);

        var domainEvent = new App.domain.entity.DomainEvent
        {
            Id = Guid.NewGuid(),
            EventType = "EmployeeDeleted",
            Data = DomainEventSerializer.Serialize(new { employee.Employee_id }),
            OccurredOn = DateTime.UtcNow,
            IsProcessed = false
        };

        await _unitOfWork.DomainEvents.AddAsync(domainEvent);
        await _unitOfWork.SaveChangesAsync();

        return Result.Success();
    }
}
