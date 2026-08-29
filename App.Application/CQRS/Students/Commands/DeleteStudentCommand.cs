using App.Application.Common;
using App.Application.Common.Events;
using App.Application.Repositories;
using MediatR;

namespace App.Application.CQRS.Students.Commands;

public record DeleteStudentCommand(string Id) : IRequest<Result>;

public class DeleteStudentCommandHandler : IRequestHandler<DeleteStudentCommand, Result>
{
    private readonly IUnitOfWork _unitOfWork;

    public DeleteStudentCommandHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<Result> Handle(DeleteStudentCommand request, CancellationToken cancellationToken)
    {
        var student = await _unitOfWork.Students.GetByIdAsync(request.Id);
        if (student == null)
            return Result.Failure("Student not found");

        await _unitOfWork.Students.DeleteAsync(student);

        var domainEvent = new App.domain.entity.DomainEvent
        {
            Id = Guid.NewGuid(),
            EventType = "StudentDeleted",
            Data = DomainEventSerializer.Serialize(new { student.Student_id }),
            OccurredOn = DateTime.UtcNow,
            IsProcessed = false
        };

        await _unitOfWork.DomainEvents.AddAsync(domainEvent);
        await _unitOfWork.SaveChangesAsync();

        return Result.Success();
    }
}
