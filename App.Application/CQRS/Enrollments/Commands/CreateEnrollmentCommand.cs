using App.Application.DTOs;
using App.Application.Common;
using App.Application.Common.Events;
using App.Application.Repositories;
using MediatR;

namespace App.Application.CQRS.Enrollments.Commands;

public record CreateEnrollmentCommand(CreateCourseStudentDto Dto) : IRequest<Result<CourseStudentDto>>;

public class CreateEnrollmentCommandHandler : IRequestHandler<CreateEnrollmentCommand, Result<CourseStudentDto>>
{
    private readonly IUnitOfWork _unitOfWork;

    public CreateEnrollmentCommandHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<CourseStudentDto>> Handle(CreateEnrollmentCommand request, CancellationToken cancellationToken)
    {
        var enrollment = new App.domain.entity.Course_student
        {
            Student_id = request.Dto.Student_id,
            Course_id = request.Dto.Course_id,
            EnrolledAt = DateTime.UtcNow
        };

        await _unitOfWork.CourseStudents.AddAsync(enrollment);

        var domainEvent = new App.domain.entity.DomainEvent
        {
            Id = Guid.NewGuid(),
            EventType = "EnrollmentCreated",
            Data = DomainEventSerializer.Serialize(new { enrollment.Student_id, enrollment.Course_id, enrollment.EnrolledAt }),
            OccurredOn = DateTime.UtcNow,
            IsProcessed = false
        };

        await _unitOfWork.DomainEvents.AddAsync(domainEvent);
        await _unitOfWork.SaveChangesAsync();

        return Result<CourseStudentDto>.Success(new CourseStudentDto
        {
            Student_id = enrollment.Student_id,
            Course_id = enrollment.Course_id,
            EnrolledAt = enrollment.EnrolledAt
        });
    }
}
