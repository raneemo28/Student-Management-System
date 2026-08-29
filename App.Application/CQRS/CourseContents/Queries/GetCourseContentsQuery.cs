using App.Application.DTOs;
using App.Application.Repositories;
using MediatR;

namespace App.Application.CQRS.CourseContents.Queries;

public record GetCourseContentsQuery(string CourseId) : IRequest<IEnumerable<CourseContentDto>>;

public class GetCourseContentsQueryHandler : IRequestHandler<GetCourseContentsQuery, IEnumerable<CourseContentDto>>
{
    private readonly ICourseContentReadRepository _readRepository;

    public GetCourseContentsQueryHandler(ICourseContentReadRepository readRepository)
    {
        _readRepository = readRepository;
    }

    public async Task<IEnumerable<CourseContentDto>> Handle(GetCourseContentsQuery request, CancellationToken cancellationToken)
    {
        var contents = await _readRepository.GetByCourseIdAsync(request.CourseId);
        return contents.Select(c => new CourseContentDto
        {
            Content_id = c.Content_id,
            Course_id = c.Course_id,
            Title = c.Title,
            Description = c.Description,
            ContentType = c.ContentType,
            FileName = c.FileName,
            FileUrl = c.FileUrl,
            FileSize = c.FileSize,
            CreatedAt = c.CreatedAt
        });
    }
}
