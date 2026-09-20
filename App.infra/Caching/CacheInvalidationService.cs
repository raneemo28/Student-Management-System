using Microsoft.Extensions.Caching.Memory;

namespace App.infra.Caching;

public interface ICacheInvalidationService
{
    Task InvalidateAsync(string key);
    Task InvalidateByEntityAsync(string entityType, string entityId);
}

public class CacheInvalidationService : ICacheInvalidationService
{
    private readonly IMemoryCache _cache;

    public CacheInvalidationService(IMemoryCache cache)
    {
        _cache = cache;
    }

    public Task InvalidateAsync(string key)
    {
        _cache.Remove(key);
        return Task.CompletedTask;
    }

    public Task InvalidateByEntityAsync(string entityType, string entityId)
    {
        var keys = new[]
        {
            $"read:{entityType}:{entityId}",
            $"read:{entityType}:all"
        };
        foreach (var key in keys)
            _cache.Remove(key);
        return Task.CompletedTask;
    }
}