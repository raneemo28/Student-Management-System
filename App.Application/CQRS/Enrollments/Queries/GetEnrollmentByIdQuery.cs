using App.Application.DTOs;
using App.Application.Repositories;
using MediatR;

namespace App.Application.CQRS.Enrollments.Queries;

public record GetEnrollmentByIdQuery(string StudentId, string CourseId) : IRequest<CourseStudentDto?>;

public class GetEnrollmentByIdQueryHandler : IRequestHandler<GetEnrollmentByIdQuery, CourseStudentDto?>
{
    private readonly IEnrollmentReadRepository _readRepository;

    public GetEnrollmentByIdQueryHandler(IEnrollmentReadRepository readRepository)
    {
        _readRepository = readRepository;
    }

    public async Task<CourseStudentDto?> Handle(GetEnrollmentByIdQuery request, CancellationToken cancellationToken)
    {
        var enrollment = await _readRepository.GetByIdAsync(request.StudentId, request.CourseId);
        if (enrollment == null) return null;

        return new CourseStudentDto
        {
            Student_id = enrollment.Student_id,
            Course_id = enrollment.Course_id,
            EnrolledAt = enrollment.EnrolledAt
        };
    }
}
