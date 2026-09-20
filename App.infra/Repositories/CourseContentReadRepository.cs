using App.Application.Repositories;
using App.domain.ReadModels;
using App.infra.Persistence;
using App.infra.Caching;
using Microsoft.EntityFrameworkCore;

namespace App.infra.Repositories;

public class CourseContentReadRepository : ICourseContentReadRepository
{
    private readonly AppReadDbContext _context;
    private readonly ICacheService _cache;

    public CourseContentReadRepository(AppReadDbContext context, ICacheService cache)
    {
        _context = context;
        _cache = cache;
    }

    public async Task<IEnumerable<CourseContentRead>> GetByCourseIdAsync(string courseId)
    {
        var key = CacheKeyGenerator.ForEntityListByForeignKey("CourseContentRead", "Course_id", courseId);
        var cached = await _cache.GetAsync<IEnumerable<CourseContentRead>>(key);
        if (cached != null)
            return cached;

        var result = await _context.CourseContentsRead
            .Where(cc => cc.Course_id == courseId)
            .ToListAsync();
        await _cache.SetAsync(key, result, CacheTtl.CourseContentRead, CancellationToken.None);
        return result;
    }

    public async Task<CourseContentRead?> GetByIdAsync(string id)
    {
        var key = CacheKeyGenerator.ForEntityById("CourseContentRead", id);
        var (found, cached) = _cache.TryGet<CourseContentRead>(key);
        if (found && cached != null)
            return cached;

        var result = await _context.CourseContentsRead.FindAsync(id);
        if (result != null)
            await _cache.SetAsync(key, result, CacheTtl.CourseContentRead, CancellationToken.None);
        return result;
    }
}