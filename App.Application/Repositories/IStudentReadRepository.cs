using App.domain.ReadModels;

namespace App.Application.Repositories;

public interface IStudentReadRepository
{
    Task<IEnumerable<StudentRead>> GetAllAsync();
    Task<StudentRead?> GetByIdAsync(string id);
}
