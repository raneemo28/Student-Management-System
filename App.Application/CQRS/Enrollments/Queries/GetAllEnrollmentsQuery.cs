using App.Application.DTOs;
using App.Application.Repositories;
using MediatR;

namespace App.Application.CQRS.Enrollments.Queries;

public record GetAllEnrollmentsQuery() : IRequest<IEnumerable<CourseStudentDto>>;

public class GetAllEnrollmentsQueryHandler : IRequestHandler<GetAllEnrollmentsQuery, IEnumerable<CourseStudentDto>>
{
    private readonly IEnrollmentReadRepository _readRepository;

    public GetAllEnrollmentsQueryHandler(IEnrollmentReadRepository readRepository)
    {
        _readRepository = readRepository;
    }

    public async Task<IEnumerable<CourseStudentDto>> Handle(GetAllEnrollmentsQuery request, CancellationToken cancellationToken)
    {
        var enrollments = await _readRepository.GetAllAsync();
        return enrollments.Select(cs => new CourseStudentDto
        {
            Student_id = cs.Student_id,
            Course_id = cs.Course_id,
            EnrolledAt = cs.EnrolledAt
        });
    }
}
