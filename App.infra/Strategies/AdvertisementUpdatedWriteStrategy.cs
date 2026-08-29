using App.Application.Common.Events;
using App.domain.ReadModels;
using App.domain.entity;
using App.infra.Persistence;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;

namespace App.infra.Strategies;

public class AdvertisementUpdatedWriteStrategy : IDomainEventWriteStrategy
{
    private readonly AppReadDbContext _readContext;

    public AdvertisementUpdatedWriteStrategy(AppReadDbContext readContext)
    {
        _readContext = readContext;
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
    }

    private record AdvertisementUpdatedData(string Advertisement_id, string Title, string Description, bool IsActive);
}
