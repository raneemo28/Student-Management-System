using App.domain.entity;

namespace App.Application.Repositories;

public interface ICourseStudentRepository : IRepository<Course_student>
{
    Task<Course_student?> GetByIdAsync(string studentId, string courseId);
}
