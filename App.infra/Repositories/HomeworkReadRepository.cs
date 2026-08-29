using App.Application.Repositories;
using App.domain.ReadModels;
using App.infra.Persistence;
using Microsoft.EntityFrameworkCore;

namespace App.infra.Repositories;

public class HomeworkReadRepository : IHomeworkReadRepository
{
    private readonly AppReadDbContext _context;

    public HomeworkReadRepository(AppReadDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<HomeworkRead>> GetByCourseIdAsync(string courseId)
    {
        return await _context.HomeworksRead
            .Where(h => h.Course_id == courseId)
            .ToListAsync();
    }

    public async Task<HomeworkRead?> GetByIdAsync(string id)
    {
        return await _context.HomeworksRead.FindAsync(id);
    }
}
