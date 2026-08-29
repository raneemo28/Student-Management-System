using App.domain.ReadModels;

namespace App.Application.Repositories;

public interface IHomeworkSolutionReadRepository
{
    Task<IEnumerable<HomeworkSolutionRead>> GetByHomeworkIdAsync(string homeworkId);
}
