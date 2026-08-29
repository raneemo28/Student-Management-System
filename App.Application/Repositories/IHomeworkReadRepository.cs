using App.domain.ReadModels;

namespace App.Application.Repositories;

public interface IHomeworkReadRepository
{
    Task<IEnumerable<HomeworkRead>> GetByCourseIdAsync(string courseId);
    Task<HomeworkRead?> GetByIdAsync(string id);
}
