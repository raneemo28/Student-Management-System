using Microsoft.Extensions.Caching.Distributed;
using System.Text.Json;

namespace App.infra.Caching;

public class RedisCacheInvalidationService : ICacheInvalidationService
{
    private readonly IDistributedCache _cache;
    private static readonly JsonSerializerOptions _jsonOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase
    };

    public RedisCacheInvalidationService(IDistributedCache cache)
    {
        _cache = cache;
    }

    public async Task InvalidateAsync(string key)
    {
        await _cache.RemoveAsync(key);
    }

    public async Task InvalidateByEntityAsync(string entityType, string entityId)
    {
        var keys = new[]
        {
            $"read:{entityType}:{entityId}",
            $"read:{entityType}:all"
        };
        foreach (var key in keys)
            await _cache.RemoveAsync(key);
    }
}