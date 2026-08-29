using App.Application.DTOs;
using App.Application.Repositories;
using MediatR;

namespace App.Application.CQRS.Homeworks.Queries;

public record GetHomeworkByIdQuery(string Id) : IRequest<HomeworkDto?>;

public class GetHomeworkByIdQueryHandler : IRequestHandler<GetHomeworkByIdQuery, HomeworkDto?>
{
    private readonly IHomeworkReadRepository _readRepository;

    public GetHomeworkByIdQueryHandler(IHomeworkReadRepository readRepository)
    {
        _readRepository = readRepository;
    }

    public async Task<HomeworkDto?> Handle(GetHomeworkByIdQuery request, CancellationToken cancellationToken)
    {
        var homework = await _readRepository.GetByIdAsync(request.Id);
        if (homework == null) return null;

        return new HomeworkDto
        {
            Homework_id = homework.Homework_id,
            Course_id = homework.Course_id,
            Title = homework.Title,
            Description = homework.Description,
            QuestionFileName = homework.QuestionFileName,
            QuestionFileUrl = homework.QuestionFileUrl,
            DueDate = homework.DueDate,
            TotalMarks = homework.TotalMarks,
            CreatedAt = homework.CreatedAt
        };
    }
}
