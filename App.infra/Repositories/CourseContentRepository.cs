using App.Application.Repositories;
using App.domain.entity;
using App.infra.Persistence;
using Microsoft.EntityFrameworkCore;

namespace App.infra.Repositories;

public class CourseContentRepository : Repository<CourseContent>, ICourseContentRepository
{
    public CourseContentRepository(AppDbContext context) : base(context) { }

    public async Task<IEnumerable<CourseContent>> GetByCourseIdAsync(string courseId)
    {
        return await _dbSet
            .Where(cc => cc.Course_id == courseId)
            .ToListAsync();
    }
}
