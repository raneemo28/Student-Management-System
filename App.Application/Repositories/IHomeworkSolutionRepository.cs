using App.domain.entity;

namespace App.Application.Repositories;

public interface IHomeworkSolutionRepository : IRepository<HomeworkSolution>
{
    Task<IEnumerable<HomeworkSolution>> GetByHomeworkIdAsync(string homeworkId);
}
