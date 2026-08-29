using App.Application.Common;
using App.Application.Common.Events;
using App.Application.Repositories;
using MediatR;

namespace App.Application.CQRS.Enrollments.Commands;

public record DeleteEnrollmentCommand(string StudentId, string CourseId) : IRequest<Result>;

public class DeleteEnrollmentCommandHandler : IRequestHandler<DeleteEnrollmentCommand, Result>
{
    private readonly IUnitOfWork _unitOfWork;

    public DeleteEnrollmentCommandHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<Result> Handle(DeleteEnrollmentCommand request, CancellationToken cancellationToken)
    {
        var enrollment = await _unitOfWork.CourseStudents.GetByIdAsync(request.StudentId, request.CourseId);
        if (enrollment == null)
            return Result.Failure("Enrollment not found");

        await _unitOfWork.CourseStudents.DeleteAsync(enrollment);

        var domainEvent = new App.domain.entity.DomainEvent
        {
            Id = Guid.NewGuid(),
            EventType = "EnrollmentDeleted",
            Data = DomainEventSerializer.Serialize(new { request.StudentId, request.CourseId }),
            OccurredOn = DateTime.UtcNow,
            IsProcessed = false
        };

        await _unitOfWork.DomainEvents.AddAsync(domainEvent);
        await _unitOfWork.SaveChangesAsync();

        return Result.Success();
    }
}
