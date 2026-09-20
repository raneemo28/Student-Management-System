using App.Application.Repositories;
using App.domain.ReadModels;
using App.infra.Persistence;
using App.infra.Caching;
using Microsoft.EntityFrameworkCore;

namespace App.infra.Repositories;

public class AdvertisementReadRepository : IAdvertisementReadRepository
{
    private readonly AppReadDbContext _context;
    private readonly ICacheService _cache;

    public AdvertisementReadRepository(AppReadDbContext context, ICacheService cache)
    {
        _context = context;
        _cache = cache;
    }

    public async Task<IEnumerable<AdvertisementRead>> GetAllAsync()
    {
        var key = CacheKeyGenerator.ForAll("AdvertisementRead");
        var cached = await _cache.GetAsync<IEnumerable<AdvertisementRead>>(key);
        if (cached != null)
            return cached;

        var result = await _context.AdvertisementsRead.ToListAsync();
        await _cache.SetAsync(key, result, CacheTtl.AdvertisementRead, CancellationToken.None);
        return result;
    }

    public async Task<AdvertisementRead?> GetByIdAsync(string id)
    {
        var key = CacheKeyGenerator.ForEntityById("AdvertisementRead", id);
        var (found, cached) = _cache.TryGet<AdvertisementRead>(key);
        if (found && cached != null)
            return cached;

        var result = await _context.AdvertisementsRead.FindAsync(id);
        if (result != null)
            await _cache.SetAsync(key, result, CacheTtl.AdvertisementRead, CancellationToken.None);
        return result;
    }
}