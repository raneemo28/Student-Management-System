using App.Application.Repositories;
using App.domain.ReadModels;
using App.infra.Persistence;
using App.infra.Caching;
using Microsoft.EntityFrameworkCore;

namespace App.infra.Repositories;

public class HomeworkReadRepository : IHomeworkReadRepository
{
    private readonly AppReadDbContext _context;
    private readonly ICacheService _cache;

    public HomeworkReadRepository(AppReadDbContext context, ICacheService cache)
    {
        _context = context;
        _cache = cache;
    }

    public async Task<IEnumerable<HomeworkRead>> GetByCourseIdAsync(string courseId)
    {
        var key = CacheKeyGenerator.ForEntityListByForeignKey("HomeworkRead", "Course_id", courseId);
        var cached = await _cache.GetAsync<IEnumerable<HomeworkRead>>(key);
        if (cached != null)
            return cached;

        var result = await _context.HomeworksRead
            .Where(h => h.Course_id == courseId)
            .ToListAsync();
        await _cache.SetAsync(key, result, CacheTtl.HomeworkRead, CancellationToken.None);
        return result;
    }

    public async Task<HomeworkRead?> GetByIdAsync(string id)
    {
        var key = CacheKeyGenerator.ForEntityById("HomeworkRead", id);
        var (found, cached) = _cache.TryGet<HomeworkRead>(key);
        if (found && cached != null)
            return cached;

        var result = await _context.HomeworksRead.FindAsync(id);
        if (result != null)
            await _cache.SetAsync(key, result, CacheTtl.HomeworkRead, CancellationToken.None);
        return result;
    }
}