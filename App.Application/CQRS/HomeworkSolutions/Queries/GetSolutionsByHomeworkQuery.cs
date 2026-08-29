using App.Application.DTOs;
using App.Application.Repositories;
using MediatR;

namespace App.Application.CQRS.HomeworkSolutions.Queries;

public record GetSolutionsByHomeworkQuery(string HomeworkId) : IRequest<IEnumerable<HomeworkSolutionDto>>;

public class GetSolutionsByHomeworkQueryHandler : IRequestHandler<GetSolutionsByHomeworkQuery, IEnumerable<HomeworkSolutionDto>>
{
    private readonly IHomeworkSolutionReadRepository _readRepository;

    public GetSolutionsByHomeworkQueryHandler(IHomeworkSolutionReadRepository readRepository)
    {
        _readRepository = readRepository;
    }

    public async Task<IEnumerable<HomeworkSolutionDto>> Handle(GetSolutionsByHomeworkQuery request, CancellationToken cancellationToken)
    {
        var solutions = await _readRepository.GetByHomeworkIdAsync(request.HomeworkId);
        return solutions.Select(s => new HomeworkSolutionDto
        {
            Solution_id = s.Solution_id,
            Homework_id = s.Homework_id,
            SolutionFileName = s.SolutionFileName,
            SolutionFileUrl = s.SolutionFileUrl,
            UploadedAt = s.UploadedAt,
            Notes = s.Notes
        });
    }
}
