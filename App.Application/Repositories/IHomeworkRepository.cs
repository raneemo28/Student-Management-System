using App.domain.entity;

namespace App.Application.Repositories;

public interface IHomeworkRepository : IRepository<Homework>
{
    Task<IEnumerable<Homework>> GetByCourseIdAsync(string courseId);
}
