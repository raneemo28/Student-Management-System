using App.Application.Repositories;
using App.domain.ReadModels;
using App.infra.Persistence;
using Microsoft.EntityFrameworkCore;

namespace App.infra.Repositories;

public class HomeworkSolutionReadRepository : IHomeworkSolutionReadRepository
{
    private readonly AppReadDbContext _context;

    public HomeworkSolutionReadRepository(AppReadDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<HomeworkSolutionRead>> GetByHomeworkIdAsync(string homeworkId)
    {
        return await _context.HomeworkSolutionsRead
            .Where(hs => hs.Homework_id == homeworkId)
            .ToListAsync();
    }
}
