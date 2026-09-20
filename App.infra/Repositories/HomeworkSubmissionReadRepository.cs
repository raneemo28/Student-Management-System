using App.Application.Repositories;
using App.domain.ReadModels;
using App.infra.Persistence;
using App.infra.Caching;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace App.infra.Repositories;

public class HomeworkSubmissionReadRepository : IHomeworkSubmissionReadRepository
{
    private readonly AppReadDbContext _context;
    private readonly ICacheService _cache;

    public HomeworkSubmissionReadRepository(AppReadDbContext context, ICacheService cache)
    {
        _context = context;
        _cache = cache;
    }

    public async Task<IEnumerable<HomeworkSubmissionRead>> GetByHomeworkIdAsync(string homeworkId)
    {
        var key = CacheKeyGenerator.ForEntityListByForeignKey("HomeworkSubmissionRead", "Homework_id", homeworkId);
        var cached = await _cache.GetAsync<IEnumerable<HomeworkSubmissionRead>>(key);
        if (cached != null)
            return cached;

        var result = await _context.HomeworkSubmissionsRead
            .Where(hs => hs.Homework_id == homeworkId)
            .ToListAsync();
        await _cache.SetAsync(key, result, CacheTtl.HomeworkSubmissionRead, CancellationToken.None);
        return result;
    }

    public async Task<HomeworkSubmissionRead?> GetByStudentAndHomeworkAsync(string studentId, string homeworkId)
    {
        var key = CacheKeyGenerator.ForEntityListByForeignKey("HomeworkSubmissionRead", "Student_id", studentId) + ":" + homeworkId;
        var (found, cached) = _cache.TryGet<HomeworkSubmissionRead>(key);
        if (found && cached != null)
            return cached;

        var result = await _context.HomeworkSubmissionsRead
            .FirstOrDefaultAsync(hs => hs.Student_id == studentId && hs.Homework_id == homeworkId);
        if (result != null)
            await _cache.SetAsync(key, result, CacheTtl.HomeworkSubmissionRead, CancellationToken.None);
        return result;
    }
}
