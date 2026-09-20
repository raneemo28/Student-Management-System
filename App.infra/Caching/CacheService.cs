using Microsoft.Extensions.Caching.Memory;

namespace App.infra.Caching;

public interface ICacheService
{
    Task<T?> GetAsync<T>(string key, CancellationToken ct = default) where T : class;
    Task SetAsync<T>(string key, T value, TimeSpan ttl, CancellationToken ct = default) where T : class;
    (bool found, T? value) TryGet<T>(string key) where T : class;
}

public class CacheService : ICacheService
{
    private readonly IMemoryCache _cache;

    public CacheService(IMemoryCache cache)
    {
        _cache = cache;
    }

    public async Task<T?> GetAsync<T>(string key, CancellationToken ct = default) where T : class
    {
        if (_cache.TryGetValue(key, out T? value) && value != null)
            return value;
        return default;
    }

    public async Task SetAsync<T>(string key, T value, TimeSpan ttl, CancellationToken ct = default) where T : class
    {
        _cache.Set(key, value, DateTimeOffset.UtcNow.Add(ttl));
    }

    public (bool found, T? value) TryGet<T>(string key) where T : class
    {
        if (_cache.TryGetValue(key, out object? raw) && raw is T typed)
            return (true, typed);
        return (false, default);
    }
}