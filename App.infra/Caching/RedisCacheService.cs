using Microsoft.Extensions.Caching.Distributed;

namespace App.infra.Caching;

public interface IRedisCacheService
{
    Task<byte[]?> GetAsync(string key, CancellationToken ct = default);
    Task SetAsync(string key, byte[] value, DistributedCacheEntryOptions options, CancellationToken ct = default);
    Task RemoveAsync(string key, CancellationToken ct = default);
}

public class RedisCacheService : IRedisCacheService
{
    private readonly IDistributedCache _cache;

    public RedisCacheService(IDistributedCache cache)
    {
        _cache = cache;
    }

    public async Task<byte[]?> GetAsync(string key, CancellationToken ct = default)
        => await _cache.GetAsync(key, ct);

    public async Task SetAsync(string key, byte[] value, DistributedCacheEntryOptions options, CancellationToken ct = default)
        => await _cache.SetAsync(key, value, options, ct);

    public async Task RemoveAsync(string key, CancellationToken ct = default)
        => await _cache.RemoveAsync(key, ct);
}
