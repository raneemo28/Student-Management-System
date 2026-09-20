using App.Application.Repositories;
using App.domain.ReadModels;
using App.infra.Persistence;
using App.infra.Caching;
using Microsoft.EntityFrameworkCore;

namespace App.infra.Repositories;

public class CourseReadRepository : ICourseReadRepository
{
    private readonly AppReadDbContext _context;
    private readonly ICacheService _cache;

    public CourseReadRepository(AppReadDbContext context, ICacheService cache)
    {
        _context = context;
        _cache = cache;
    }

    public async Task<IEnumerable<CourseRead>> GetAllAsync()
    {
        var key = CacheKeyGenerator.ForAll("CourseRead");
        var cached = await _cache.GetAsync<IEnumerable<CourseRead>>(key);
        if (cached != null)
            return cached;

        var result = await _context.CoursesRead.ToListAsync();
        await _cache.SetAsync(key, result, CacheTtl.CourseRead, CancellationToken.None);
        return result;
    }

    public async Task<CourseRead?> GetByIdAsync(string id)
    {
        var key = CacheKeyGenerator.ForEntityById("CourseRead", id);
        var (found, cached) = _cache.TryGet<CourseRead>(key);
        if (found && cached != null)
            return cached;

        var result = await _context.CoursesRead.FindAsync(id);
        if (result != null)
            await _cache.SetAsync(key, result, CacheTtl.CourseRead, CancellationToken.None);
        return result;
    }
}