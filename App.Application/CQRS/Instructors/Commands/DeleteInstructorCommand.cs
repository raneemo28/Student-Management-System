using App.Application.Common;
using App.Application.Common.Events;
using App.Application.Repositories;
using MediatR;

namespace App.Application.CQRS.Instructors.Commands;

public record DeleteInstructorCommand(string Id) : IRequest<Result>;

public class DeleteInstructorCommandHandler : IRequestHandler<DeleteInstructorCommand, Result>
{
    private readonly IUnitOfWork _unitOfWork;

    public DeleteInstructorCommandHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<Result> Handle(DeleteInstructorCommand request, CancellationToken cancellationToken)
    {
        var instructor = await _unitOfWork.Instructors.GetByIdAsync(request.Id);
        if (instructor == null)
            return Result.Failure("Instructor not found");

        await _unitOfWork.Instructors.DeleteAsync(instructor);

        var domainEvent = new App.domain.entity.DomainEvent
        {
            Id = Guid.NewGuid(),
            EventType = "InstructorDeleted",
            Data = DomainEventSerializer.Serialize(new { instructor.Instructor_id }),
            OccurredOn = DateTime.UtcNow,
            IsProcessed = false
        };

        await _unitOfWork.DomainEvents.AddAsync(domainEvent);
        await _unitOfWork.SaveChangesAsync();

        return Result.Success();
    }
}
