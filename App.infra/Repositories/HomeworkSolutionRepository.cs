using App.Application.Repositories;
using App.domain.entity;
using App.infra.Persistence;
using Microsoft.EntityFrameworkCore;

namespace App.infra.Repositories;

public class HomeworkSolutionRepository : Repository<HomeworkSolution>, IHomeworkSolutionRepository
{
    public HomeworkSolutionRepository(AppDbContext context) : base(context) { }

    public async Task<IEnumerable<HomeworkSolution>> GetByHomeworkIdAsync(string homeworkId)
    {
        return await _dbSet
            .Where(hs => hs.Homework_id == homeworkId)
            .ToListAsync();
    }
}
