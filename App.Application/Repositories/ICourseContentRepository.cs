using App.domain.entity;

namespace App.Application.Repositories;

public interface ICourseContentRepository : IRepository<CourseContent>
{
    Task<IEnumerable<CourseContent>> GetByCourseIdAsync(string courseId);
}
