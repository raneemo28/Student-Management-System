using App.domain.ReadModels;

namespace App.Application.Repositories;

public interface ICourseContentReadRepository
{
    Task<IEnumerable<CourseContentRead>> GetByCourseIdAsync(string courseId);
    Task<CourseContentRead?> GetByIdAsync(string id);
}
