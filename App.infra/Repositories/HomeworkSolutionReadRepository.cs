using App.Application.Repositories;
using App.domain.ReadModels;
using App.infra.Persistence;
using App.infra.Caching;
using Microsoft.EntityFrameworkCore;

namespace App.infra.Repositories;

public class HomeworkSolutionReadRepository : IHomeworkSolutionReadRepository
{
    private readonly AppReadDbContext _context;
    private readonly ICacheService _cache;

    public HomeworkSolutionReadRepository(AppReadDbContext context, ICacheService cache)
    {
        _context = context;
        _cache = cache;
    }

    public async Task<IEnumerable<HomeworkSolutionRead>> GetByHomeworkIdAsync(string homeworkId)
    {
        var key = CacheKeyGenerator.ForEntityListByForeignKey("HomeworkSolutionRead", "Homework_id", homeworkId);
        var cached = await _cache.GetAsync<IEnumerable<HomeworkSolutionRead>>(key);
        if (cached != null)
            return cached;

        var result = await _context.HomeworkSolutionsRead
            .Where(hs => hs.Homework_id == homeworkId)
            .ToListAsync();
        await _cache.SetAsync(key, result, CacheTtl.HomeworkSolutionRead, CancellationToken.None);
        return result;
    }
}
