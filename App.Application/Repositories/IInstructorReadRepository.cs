using App.domain.ReadModels;

namespace App.Application.Repositories;

public interface IInstructorReadRepository
{
    Task<IEnumerable<InstructorRead>> GetAllAsync();
    Task<InstructorRead?> GetByIdAsync(string id);
}
