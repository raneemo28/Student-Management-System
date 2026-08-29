using App.Application.Repositories;
using App.domain.ReadModels;
using App.infra.Persistence;
using Microsoft.EntityFrameworkCore;

namespace App.infra.Repositories;

public class InstructorReadRepository : IInstructorReadRepository
{
    private readonly AppReadDbContext _context;

    public InstructorReadRepository(AppReadDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<InstructorRead>> GetAllAsync()
    {
        return await _context.InstructorsRead.ToListAsync();
    }

    public async Task<InstructorRead?> GetByIdAsync(string id)
    {
        return await _context.InstructorsRead.FindAsync(id);
    }
}
