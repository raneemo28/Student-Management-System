using App.Application.Repositories;
using App.domain.ReadModels;
using App.infra.Persistence;
using Microsoft.EntityFrameworkCore;

namespace App.infra.Repositories;

public class EnrollmentReadRepository : IEnrollmentReadRepository
{
    private readonly AppReadDbContext _context;

    public EnrollmentReadRepository(AppReadDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<CourseStudentRead>> GetAllAsync()
    {
        return await _context.EnrollmentsRead.ToListAsync();
    }

    public async Task<CourseStudentRead?> GetByIdAsync(string studentId, string courseId)
    {
        return await _context.EnrollmentsRead
            .FirstOrDefaultAsync(cs => cs.Student_id == studentId && cs.Course_id == courseId);
    }
}
