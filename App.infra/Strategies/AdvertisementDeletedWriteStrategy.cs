using App.Application.Common.Events;
using App.domain.ReadModels;
using App.domain.entity;
using App.infra.Persistence;
using App.infra.Caching;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;

namespace App.infra.Strategies;

public class AdvertisementDeletedWriteStrategy : IDomainEventWriteStrategy
{
    private readonly AppReadDbContext _readContext;
    private readonly ICacheInvalidationService _cacheInvalidation;

    public AdvertisementDeletedWriteStrategy(AppReadDbContext readContext, ICacheInvalidationService cacheInvalidation)
    {
        _readContext = readContext;
        _cacheInvalidation = cacheInvalidation;
    }

    public async Task ProcessAsync(DomainEvent domainEvent)
    {
        var data = JsonSerializer.Deserialize<AdvertisementDeletedData>(domainEvent.Data);
        if (data == null) return;

        var advertisementRead = await _readContext.AdvertisementsRead.FindAsync(data.Advertisement_id);
        if (advertisementRead == null) return;

        _readContext.AdvertisementsRead.Remove(advertisementRead);
        await _readContext.SaveChangesAsync();
        await _cacheInvalidation.InvalidateByEntityAsync("AdvertisementRead", data.Advertisement_id);
    }

    private record AdvertisementDeletedData(string Advertisement_id);
}
