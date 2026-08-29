using App.Application.DTOs;
using App.Application.Common;
using App.Application.Common.Events;
using App.Application.Repositories;
using MediatR;

namespace App.Application.CQRS.Courses.Commands;

public record UpdateCourseCommand(string Id, UpdateCourseDto Dto) : IRequest<Result<CourseDto>>;

public class UpdateCourseCommandHandler : IRequestHandler<UpdateCourseCommand, Result<CourseDto>>
{
    private readonly IUnitOfWork _unitOfWork;

    public UpdateCourseCommandHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<CourseDto>> Handle(UpdateCourseCommand request, CancellationToken cancellationToken)
    {
        var course = await _unitOfWork.Courses.GetByIdAsync(request.Id);
        if (course == null)
            return Result<CourseDto>.Failure("Course not found");

        course.Course_name = request.Dto.Course_name;
        course.Instructor_id = request.Dto.Instructor_id ?? course.Instructor_id;
        await _unitOfWork.Courses.UpdateAsync(course);

        var domainEvent = new App.domain.entity.DomainEvent
        {
            Id = Guid.NewGuid(),
            EventType = "CourseUpdated",
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
