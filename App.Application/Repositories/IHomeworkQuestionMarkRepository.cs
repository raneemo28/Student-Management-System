using App.domain.entity;

namespace App.Application.Repositories;

public interface IHomeworkQuestionMarkRepository : IRepository<HomeworkQuestionMark>
{
    Task<IEnumerable<HomeworkQuestionMark>> GetBySubmissionIdAsync(string submissionId);
}
