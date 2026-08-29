using App.Application.Repositories;
using App.domain.ReadModels;
using App.infra.Persistence;
using Microsoft.EntityFrameworkCore;

namespace App.infra.Repositories;

public class StudentReadRepository : IStudentReadRepository
{
    private readonly AppReadDbContext _context;

    public StudentReadRepository(AppReadDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<StudentRead>> GetAllAsync()
    {
        return await _context.StudentsRead.ToListAsync();
    }

    public async Task<StudentRead?> GetByIdAsync(string id)
    {
        return await _context.StudentsRead.FindAsync(id);
    }
}
