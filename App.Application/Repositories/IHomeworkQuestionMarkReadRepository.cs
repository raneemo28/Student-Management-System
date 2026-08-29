using App.domain.ReadModels;

namespace App.Application.Repositories;

public interface IHomeworkQuestionMarkReadRepository
{
    Task<IEnumerable<HomeworkQuestionMarkRead>> GetBySubmissionIdAsync(string submissionId);
}
