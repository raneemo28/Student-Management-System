using App.domain.ReadModels;

namespace App.Application.Repositories;

public interface ICourseReadRepository
{
    Task<IEnumerable<CourseRead>> GetAllAsync();
    Task<CourseRead?> GetByIdAsync(string id);
}
