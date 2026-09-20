using App.Application.Repositories;
using App.domain.ReadModels;
using App.infra.Persistence;
using App.infra.Caching;
using Microsoft.EntityFrameworkCore;

namespace App.infra.Repositories;

public class StudentReadRepository : IStudentReadRepository
{
    private readonly AppReadDbContext _context;
    private readonly ICacheService _cache;

    public StudentReadRepository(AppReadDbContext context, ICacheService cache)
    {
        _context = context;
        _cache = cache;
    }

    public async Task<IEnumerable<StudentRead>> GetAllAsync()
    {
        var key = CacheKeyGenerator.ForAll("StudentRead");
        var cached = await _cache.GetAsync<IEnumerable<StudentRead>>(key);
        if (cached != null)
            return cached;

        var result = await _context.StudentsRead.ToListAsync();
        await _cache.SetAsync(key, result, CacheTtl.StudentRead, CancellationToken.None);
        return result;
    }

    public async Task<StudentRead?> GetByIdAsync(string id)
    {
        var key = CacheKeyGenerator.ForEntityById("StudentRead", id);
        var (found, cached) = _cache.TryGet<StudentRead>(key);
        if (found && cached != null)
            return cached;

        var result = await _context.StudentsRead.FindAsync(id);
        if (result != null)
            await _cache.SetAsync(key, result, CacheTtl.StudentRead, CancellationToken.None);
        return result;
    }
}