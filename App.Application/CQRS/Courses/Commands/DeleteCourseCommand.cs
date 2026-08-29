using App.Application.Common;
using App.Application.Common.Events;
using App.Application.Repositories;
using MediatR;

namespace App.Application.CQRS.Courses.Commands;

public record DeleteCourseCommand(string Id) : IRequest<Result>;

public class DeleteCourseCommandHandler : IRequestHandler<DeleteCourseCommand, Result>
{
    private readonly IUnitOfWork _unitOfWork;

    public DeleteCourseCommandHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<Result> Handle(DeleteCourseCommand request, CancellationToken cancellationToken)
    {
        var course = await _unitOfWork.Courses.GetByIdAsync(request.Id);
        if (course == null)
            return Result.Failure("Course not found");

        await _unitOfWork.Courses.DeleteAsync(course);

        var domainEvent = new App.domain.entity.DomainEvent
        {
            Id = Guid.NewGuid(),
            EventType = "CourseDeleted",
            Data = DomainEventSerializer.Serialize(new { course.Course_id }),
            OccurredOn = DateTime.UtcNow,
            IsProcessed = false
        };

        await _unitOfWork.DomainEvents.AddAsync(domainEvent);
        await _unitOfWork.SaveChangesAsync();

        return Result.Success();
    }
}
