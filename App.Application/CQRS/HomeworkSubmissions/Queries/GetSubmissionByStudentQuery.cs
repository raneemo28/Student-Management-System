using App.Application.DTOs;
using App.Application.Repositories;
using MediatR;

namespace App.Application.CQRS.HomeworkSubmissions.Queries;

public record GetSubmissionByStudentQuery(string StudentId, string HomeworkId) : IRequest<HomeworkSubmissionDto?>;

public class GetSubmissionByStudentQueryHandler : IRequestHandler<GetSubmissionByStudentQuery, HomeworkSubmissionDto?>
{
    private readonly IHomeworkSubmissionReadRepository _readRepository;

    public GetSubmissionByStudentQueryHandler(IHomeworkSubmissionReadRepository readRepository)
    {
        _readRepository = readRepository;
    }

    public async Task<HomeworkSubmissionDto?> Handle(GetSubmissionByStudentQuery request, CancellationToken cancellationToken)
    {
        var submission = await _readRepository.GetByStudentAndHomeworkAsync(request.StudentId, request.HomeworkId);
        if (submission == null) return null;

        return new HomeworkSubmissionDto
        {
            Submission_id = submission.Submission_id,
            Homework_id = submission.Homework_id,
            Student_id = submission.Student_id,
            SolutionFileName = submission.SolutionFileName,
            SolutionFileUrl = submission.SolutionFileUrl,
            SubmittedAt = submission.SubmittedAt,
            Mark = submission.Mark
        };
    }
}
