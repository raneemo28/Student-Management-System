using App.Application.DTOs;
using App.Application.Repositories;
using MediatR;

namespace App.Application.CQRS.HomeworkSubmissions.Queries;

public record GetSubmissionsByHomeworkQuery(string HomeworkId) : IRequest<IEnumerable<HomeworkSubmissionDto>>;

public class GetSubmissionsByHomeworkQueryHandler : IRequestHandler<GetSubmissionsByHomeworkQuery, IEnumerable<HomeworkSubmissionDto>>
{
    private readonly IHomeworkSubmissionReadRepository _readRepository;

    public GetSubmissionsByHomeworkQueryHandler(IHomeworkSubmissionReadRepository readRepository)
    {
        _readRepository = readRepository;
    }

    public async Task<IEnumerable<HomeworkSubmissionDto>> Handle(GetSubmissionsByHomeworkQuery request, CancellationToken cancellationToken)
    {
        var submissions = await _readRepository.GetByHomeworkIdAsync(request.HomeworkId);
        return submissions.Select(s => new HomeworkSubmissionDto
        {
            Submission_id = s.Submission_id,
            Homework_id = s.Homework_id,
            Student_id = s.Student_id,
            SolutionFileName = s.SolutionFileName,
            SolutionFileUrl = s.SolutionFileUrl,
            SubmittedAt = s.SubmittedAt,
            Mark = s.Mark
        });
    }
}
