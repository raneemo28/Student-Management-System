using App.Application.Repositories;
using App.domain.ReadModels;
using App.infra.Persistence;
using App.infra.Caching;
using Microsoft.EntityFrameworkCore;

namespace App.infra.Repositories;

public class EnrollmentReadRepository : IEnrollmentReadRepository
{
    private readonly AppReadDbContext _context;
    private readonly ICacheService _cache;

    public EnrollmentReadRepository(AppReadDbContext context, ICacheService cache)
    {
        _context = context;
        _cache = cache;
    }

    public async Task<IEnumerable<CourseStudentRead>> GetAllAsync()
    {
        var key = CacheKeyGenerator.ForAll("CourseStudentRead");
        var cached = await _cache.GetAsync<IEnumerable<CourseStudentRead>>(key);
        if (cached != null)
            return cached;

        var result = await _context.EnrollmentsRead.ToListAsync();
        await _cache.SetAsync(key, result, CacheTtl.CourseStudentRead, CancellationToken.None);
        return result;
    }

    public async Task<CourseStudentRead?> GetByIdAsync(string studentId, string courseId)
    {
        var key = CacheKeyGenerator.ForEntityListByForeignKey("CourseStudentRead", "Student_id", studentId) + ":" + courseId;
        var (found, cached) = _cache.TryGet<CourseStudentRead>(key);
        if (found && cached != null)
            return cached;

        var result = await _context.EnrollmentsRead
            .FirstOrDefaultAsync(cs => cs.Student_id == studentId && cs.Course_id == courseId);
        if (result != null)
            await _cache.SetAsync(key, result, CacheTtl.CourseStudentRead, CancellationToken.None);
        return result;
    }
}