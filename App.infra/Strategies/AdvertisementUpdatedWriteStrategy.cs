using App.Application.Common.Events;
using App.domain.ReadModels;
using App.domain.entity;
using App.infra.Persistence;
using App.infra.Caching;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;

namespace App.infra.Strategies;

public class AdvertisementUpdatedWriteStrategy : IDomainEventWriteStrategy
{
    private readonly AppReadDbContext _readContext;
    private readonly ICacheInvalidationService _cacheInvalidation;

    public AdvertisementUpdatedWriteStrategy(AppReadDbContext readContext, ICacheInvalidationService cacheInvalidation)
    {
        _readContext = readContext;
        _cacheInvalidation = cacheInvalidation;
    }

    public async Task ProcessAsync(DomainEvent domainEvent)
    {
        var data = JsonSerializer.Deserialize<AdvertisementUpdatedData>(domainEvent.Data);
        if (data == null) return;

        var advertisementRead = await _readContext.AdvertisementsRead.FindAsync(data.Advertisement_id);
        if (advertisementRead == null) return;

        advertisementRead.Title = data.Title;
        advertisementRead.Description = data.Description;
        advertisementRead.IsActive = data.IsActive;
        await _readContext.SaveChangesAsync();
        await _cacheInvalidation.InvalidateByEntityAsync("AdvertisementRead", data.Advertisement_id);
    }

    private record AdvertisementUpdatedData(string Advertisement_id, string Title, string Description, bool IsActive);
}
