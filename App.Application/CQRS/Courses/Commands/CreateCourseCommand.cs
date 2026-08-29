using App.Application.DTOs;
using App.Application.Common;
using App.Application.Common.Events;
using App.Application.Repositories;
using MediatR;

namespace App.Application.CQRS.Courses.Commands;

public record CreateCourseCommand(CreateCourseDto Dto) : IRequest<Result<CourseDto>>;

public class CreateCourseCommandHandler : IRequestHandler<CreateCourseCommand, Result<CourseDto>>
{
    private readonly IUnitOfWork _unitOfWork;

    public CreateCourseCommandHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<CourseDto>> Handle(CreateCourseCommand request, CancellationToken cancellationToken)
    {
        var course = new App.domain.entity.Course
        {
            Course_name = request.Dto.Course_name,
            Course_id = Guid.NewGuid().ToString(),
            Instructor_id = request.Dto.Instructor_id
        };

        await _unitOfWork.Courses.AddAsync(course);

        var domainEvent = new App.domain.entity.DomainEvent
        {
            Id = Guid.NewGuid(),
            EventType = "CourseCreated",
            Data = DomainEventSerializer.Serialize(new { course.Course_id, course.Course_name, course.Instructor_id }),
            OccurredOn = DateTime.UtcNow,
            IsProcessed = false
        };

        await _unitOfWork.DomainEvents.AddAsync(domainEvent);
        await _unitOfWork.SaveChangesAsync();

        return Result<CourseDto>.Success(new CourseDto
        {
            Course_id = course.Course_id,
            Course_name = course.Course_name,
            Instructor_id = course.Instructor_id
        });
    }
}
