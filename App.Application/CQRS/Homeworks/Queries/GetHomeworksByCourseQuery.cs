using App.Application.DTOs;
using App.Application.Repositories;
using MediatR;

namespace App.Application.CQRS.Homeworks.Queries;

public record GetHomeworksByCourseQuery(string CourseId) : IRequest<IEnumerable<HomeworkDto>>;

public class GetHomeworksByCourseQueryHandler : IRequestHandler<GetHomeworksByCourseQuery, IEnumerable<HomeworkDto>>
{
    private readonly IHomeworkReadRepository _readRepository;

    public GetHomeworksByCourseQueryHandler(IHomeworkReadRepository readRepository)
    {
        _readRepository = readRepository;
    }

    public async Task<IEnumerable<HomeworkDto>> Handle(GetHomeworksByCourseQuery request, CancellationToken cancellationToken)
    {
        var homeworks = await _readRepository.GetByCourseIdAsync(request.CourseId);
        return homeworks.Select(h => new HomeworkDto
        {
            Homework_id = h.Homework_id,
            Course_id = h.Course_id,
            Title = h.Title,
            Description = h.Description,
            QuestionFileName = h.QuestionFileName,
            QuestionFileUrl = h.QuestionFileUrl,
            DueDate = h.DueDate,
            TotalMarks = h.TotalMarks,
            CreatedAt = h.CreatedAt
        });
    }
}
