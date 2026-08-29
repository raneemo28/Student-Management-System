using App.Application.Repositories;
using App.domain.entity;
using App.infra.Persistence;
using Microsoft.EntityFrameworkCore;

namespace App.infra.Repositories;

public class CourseStudentRepository : Repository<Course_student>, ICourseStudentRepository
{
    public CourseStudentRepository(AppDbContext context) : base(context) { }

    public async Task<Course_student?> GetByIdAsync(string studentId, string courseId)
    {
        return await _dbSet
            .FirstOrDefaultAsync(cs => cs.Student_id == studentId && cs.Course_id == courseId);
    }
}
