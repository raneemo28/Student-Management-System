using App.domain.ReadModels;

namespace App.Application.Repositories;

public interface IHomeworkSubmissionReadRepository
{
    Task<IEnumerable<HomeworkSubmissionRead>> GetByHomeworkIdAsync(string homeworkId);
    Task<HomeworkSubmissionRead?> GetByStudentAndHomeworkAsync(string studentId, string homeworkId);
}
