using App.Application.Repositories;
using App.domain.ReadModels;
using App.infra.Persistence;
using Microsoft.EntityFrameworkCore;

namespace App.infra.Repositories;

public class CourseReadRepository : ICourseReadRepository
{
    private readonly AppReadDbContext _context;

    public CourseReadRepository(AppReadDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<CourseRead>> GetAllAsync()
    {
        return await _context.CoursesRead.ToListAsync();
    }

    public async Task<CourseRead?> GetByIdAsync(string id)
    {
        return await _context.CoursesRead.FindAsync(id);
    }
}
