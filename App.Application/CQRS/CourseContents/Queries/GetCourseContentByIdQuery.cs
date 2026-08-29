using App.Application.DTOs;
using App.Application.Repositories;
using MediatR;

namespace App.Application.CQRS.CourseContents.Queries;

public record GetCourseContentByIdQuery(string Id) : IRequest<CourseContentDto?>;

public class GetCourseContentByIdQueryHandler : IRequestHandler<GetCourseContentByIdQuery, CourseContentDto?>
{
    private readonly ICourseContentReadRepository _readRepository;

    public GetCourseContentByIdQueryHandler(ICourseContentReadRepository readRepository)
    {
        _readRepository = readRepository;
    }

    public async Task<CourseContentDto?> Handle(GetCourseContentByIdQuery request, CancellationToken cancellationToken)
    {
        var content = await _readRepository.GetByIdAsync(request.Id);
        if (content == null) return null;

        return new CourseContentDto
        {
            Content_id = content.Content_id,
            Course_id = content.Course_id,
            Title = content.Title,
            Description = content.Description,
            ContentType = content.ContentType,
            FileName = content.FileName,
            FileUrl = content.FileUrl,
            FileSize = content.FileSize,
            CreatedAt = content.CreatedAt
        };
    }
}
