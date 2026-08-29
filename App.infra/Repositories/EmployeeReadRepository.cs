using App.Application.Repositories;
using App.domain.ReadModels;
using App.infra.Persistence;
using Microsoft.EntityFrameworkCore;

namespace App.infra.Repositories;

public class EmployeeReadRepository : IEmployeeReadRepository
{
    private readonly AppReadDbContext _context;

    public EmployeeReadRepository(AppReadDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<EmployeeRead>> GetAllAsync()
    {
        return await _context.EmployeesRead.ToListAsync();
    }

    public async Task<EmployeeRead?> GetByIdAsync(string id)
    {
        return await _context.EmployeesRead.FindAsync(id);
    }
}
