using App.domain.ReadModels;

namespace App.Application.Repositories;

public interface IEnrollmentReadRepository
{
    Task<IEnumerable<CourseStudentRead>> GetAllAsync();
    Task<CourseStudentRead?> GetByIdAsync(string studentId, string courseId);
}
