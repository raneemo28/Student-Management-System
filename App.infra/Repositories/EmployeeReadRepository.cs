using App.Application.Repositories;
using App.domain.ReadModels;
using App.infra.Persistence;
using App.infra.Caching;
using Microsoft.EntityFrameworkCore;

namespace App.infra.Repositories;

public class EmployeeReadRepository : IEmployeeReadRepository
{
    private readonly AppReadDbContext _context;
    private readonly ICacheService _cache;

    public EmployeeReadRepository(AppReadDbContext context, ICacheService cache)
    {
        _context = context;
        _cache = cache;
    }

    public async Task<IEnumerable<EmployeeRead>> GetAllAsync()
    {
        var key = CacheKeyGenerator.ForAll("EmployeeRead");
        var cached = await _cache.GetAsync<IEnumerable<EmployeeRead>>(key);
        if (cached != null)
            return cached;

        var result = await _context.EmployeesRead.ToListAsync();
        await _cache.SetAsync(key, result, CacheTtl.EmployeeRead, CancellationToken.None);
        return result;
    }

    public async Task<EmployeeRead?> GetByIdAsync(string id)
    {
        var key = CacheKeyGenerator.ForEntityById("EmployeeRead", id);
        var (found, cached) = _cache.TryGet<EmployeeRead>(key);
        if (found && cached != null)
            return cached;

        var result = await _context.EmployeesRead.FindAsync(id);
        if (result != null)
            await _cache.SetAsync(key, result, CacheTtl.EmployeeRead, CancellationToken.None);
        return result;
    }
}