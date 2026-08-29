using App.Application.Repositories;
using App.domain.ReadModels;
using App.infra.Persistence;
using Microsoft.EntityFrameworkCore;

namespace App.infra.Repositories;

public class CourseContentReadRepository : ICourseContentReadRepository
{
    private readonly AppReadDbContext _context;

    public CourseContentReadRepository(AppReadDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<CourseContentRead>> GetByCourseIdAsync(string courseId)
    {
        return await _context.CourseContentsRead
            .Where(cc => cc.Course_id == courseId)
            .ToListAsync();
    }

    public async Task<CourseContentRead?> GetByIdAsync(string id)
    {
        return await _context.CourseContentsRead.FindAsync(id);
    }
}
