using App.Application.Repositories;
using App.domain.ReadModels;
using App.infra.Persistence;
using App.infra.Caching;
using Microsoft.EntityFrameworkCore;

namespace App.infra.Repositories;

public class HomeworkQuestionMarkReadRepository : IHomeworkQuestionMarkReadRepository
{
    private readonly AppReadDbContext _context;
    private readonly ICacheService _cache;

    public HomeworkQuestionMarkReadRepository(AppReadDbContext context, ICacheService cache)
    {
        _context = context;
        _cache = cache;
    }

    public async Task<IEnumerable<HomeworkQuestionMarkRead>> GetBySubmissionIdAsync(string submissionId)
    {
        var key = CacheKeyGenerator.ForEntityListByForeignKey("HomeworkQuestionMarkRead", "Submission_id", submissionId);
        var cached = await _cache.GetAsync<IEnumerable<HomeworkQuestionMarkRead>>(key);
        if (cached != null)
            return cached;

        var result = await _context.HomeworkQuestionMarksRead
            .Where(hqm => hqm.Submission_id == submissionId)
            .ToListAsync();
        await _cache.SetAsync(key, result, CacheTtl.HomeworkQuestionMarkRead, CancellationToken.None);
        return result;
    }
}