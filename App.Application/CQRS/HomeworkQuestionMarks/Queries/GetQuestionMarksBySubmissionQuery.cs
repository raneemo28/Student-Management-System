using App.Application.DTOs;
using App.Application.Repositories;
using MediatR;

namespace App.Application.CQRS.HomeworkQuestionMarks.Queries;

public record GetQuestionMarksBySubmissionQuery(string SubmissionId) : IRequest<IEnumerable<HomeworkQuestionMarkDto>>;

public class GetQuestionMarksBySubmissionQueryHandler : IRequestHandler<GetQuestionMarksBySubmissionQuery, IEnumerable<HomeworkQuestionMarkDto>>
{
    private readonly IHomeworkQuestionMarkReadRepository _readRepository;

    public GetQuestionMarksBySubmissionQueryHandler(IHomeworkQuestionMarkReadRepository readRepository)
    {
        _readRepository = readRepository;
    }

    public async Task<IEnumerable<HomeworkQuestionMarkDto>> Handle(GetQuestionMarksBySubmissionQuery request, CancellationToken cancellationToken)
    {
        var questionMarks = await _readRepository.GetBySubmissionIdAsync(request.SubmissionId);
        return questionMarks.Select(qm => new HomeworkQuestionMarkDto
        {
            QuestionMark_id = qm.QuestionMark_id,
            Submission_id = qm.Submission_id,
            QuestionNumber = qm.QuestionNumber,
            QuestionDescription = qm.QuestionDescription,
            MaxMarks = qm.MaxMarks,
            ObtainedMarks = qm.ObtainedMarks
        });
    }
}
