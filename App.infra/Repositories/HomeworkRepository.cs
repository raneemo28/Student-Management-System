using App.Application.Repositories;
using App.domain.entity;
using App.infra.Persistence;
using Microsoft.EntityFrameworkCore;

namespace App.infra.Repositories;

public class HomeworkRepository : Repository<Homework>, IHomeworkRepository
{
    public HomeworkRepository(AppDbContext context) : base(context) { }

    public async Task<IEnumerable<Homework>> GetByCourseIdAsync(string courseId)
    {
        return await _dbSet
            .Where(h => h.Course_id == courseId)
            .ToListAsync();
    }
}
