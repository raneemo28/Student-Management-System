using App.Application.Repositories;
using App.domain.ReadModels;
using App.infra.Persistence;
using App.infra.Caching;
using Microsoft.EntityFrameworkCore;

namespace App.infra.Repositories;

public class InstructorReadRepository : IInstructorReadRepository
{
    private readonly AppReadDbContext _context;
    private readonly ICacheService _cache;

    public InstructorReadRepository(AppReadDbContext context, ICacheService cache)
    {
        _context = context;
        _cache = cache;
    }

    public async Task<IEnumerable<InstructorRead>> GetAllAsync()
    {
        var key = CacheKeyGenerator.ForAll("InstructorRead");
        var cached = await _cache.GetAsync<IEnumerable<InstructorRead>>(key);
        if (cached != null)
            return cached;

        var result = await _context.InstructorsRead.ToListAsync();
        await _cache.SetAsync(key, result, CacheTtl.InstructorRead, CancellationToken.None);
        return result;
    }

    public async Task<InstructorRead?> GetByIdAsync(string id)
    {
        var key = CacheKeyGenerator.ForEntityById("InstructorRead", id);
        var (found, cached) = _cache.TryGet<InstructorRead>(key);
        if (found && cached != null)
            return cached;

        var result = await _context.InstructorsRead.FindAsync(id);
        if (result != null)
            await _cache.SetAsync(key, result, CacheTtl.InstructorRead, CancellationToken.None);
        return result;
    }
}
