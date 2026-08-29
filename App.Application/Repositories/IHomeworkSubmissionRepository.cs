using App.domain.entity;

namespace App.Application.Repositories;

public interface IHomeworkSubmissionRepository : IRepository<HomeworkSubmission>
{
    Task<IEnumerable<HomeworkSubmission>> GetByHomeworkIdAsync(string homeworkId);
    Task<HomeworkSubmission?> GetByStudentAndHomeworkAsync(string studentId, string homeworkId);
}
