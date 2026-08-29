using App.Application.DTOs;
using App.Application.Repositories;
using MediatR;

namespace App.Application.CQRS.Courses.Queries;

public record GetAllCoursesQuery() : IRequest<IEnumerable<CourseDto>>;

public class GetAllCoursesQueryHandler : IRequestHandler<GetAllCoursesQuery, IEnumerable<CourseDto>>
{
    private readonly ICourseReadRepository _readRepository;

    public GetAllCoursesQueryHandler(ICourseReadRepository readRepository)
    {
        _readRepository = readRepository;
    }

    public async Task<IEnumerable<CourseDto>> Handle(GetAllCoursesQuery request, CancellationToken cancellationToken)
    {
        var courses = await _readRepository.GetAllAsync();
        return courses.Select(c => new CourseDto
        {
            Course_id = c.Course_id,
            Course_name = c.Course_name
        });
    }
}
