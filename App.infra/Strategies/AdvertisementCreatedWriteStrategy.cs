using App.Application.Common.Events;
using App.domain.ReadModels;
using App.domain.entity;
using App.infra.Persistence;
using App.infra.Caching;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;

namespace App.infra.Strategies;

public class AdvertisementCreatedWriteStrategy : IDomainEventWriteStrategy
{
    private readonly AppReadDbContext _readContext;
    private readonly ICacheInvalidationService _cacheInvalidation;

    public AdvertisementCreatedWriteStrategy(AppReadDbContext readContext, ICacheInvalidationService cacheInvalidation)
    {
        _readContext = readContext;
        _cacheInvalidation = cacheInvalidation;
    }

    public async Task ProcessAsync(DomainEvent domainEvent)
    {
        var data = JsonSerializer.Deserialize<AdvertisementCreatedData>(domainEvent.Data);
        if (data == null) return;

        var advertisementRead = new AdvertisementRead
        {
            Advertisement_id = data.Advertisement_id,
            Title = data.Title,
            Description = data.Description,
            PublishedAt = data.PublishedAt,
            IsActive = data.IsActive
        };

        _readContext.AdvertisementsRead.Add(advertisementRead);
        await _readContext.SaveChangesAsync();
        await _cacheInvalidation.InvalidateByEntityAsync("AdvertisementRead", data.Advertisement_id);
    }

    private record AdvertisementCreatedData(string Advertisement_id, string Title, string Description, DateTime PublishedAt, bool IsActive);
}
