using App.Application.DTOs;
using App.Application.Repositories;
using MediatR;

namespace App.Application.CQRS.Courses.Queries;

public record GetCourseByIdQuery(string Id) : IRequest<CourseDto?>;

public class GetCourseByIdQueryHandler : IRequestHandler<GetCourseByIdQuery, CourseDto?>
{
    private readonly ICourseReadRepository _readRepository;

    public GetCourseByIdQueryHandler(ICourseReadRepository readRepository)
    {
        _readRepository = readRepository;
    }

    public async Task<CourseDto?> Handle(GetCourseByIdQuery request, CancellationToken cancellationToken)
    {
        var course = await _readRepository.GetByIdAsync(request.Id);
        if (course == null) return null;

        return new CourseDto
        {
            Course_id = course.Course_id,
            Course_name = course.Course_name
        };
    }
}
