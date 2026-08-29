using App.domain.ReadModels;

namespace App.Application.Repositories;

public interface IEmployeeReadRepository
{
    Task<IEnumerable<EmployeeRead>> GetAllAsync();
    Task<EmployeeRead?> GetByIdAsync(string id);
}
