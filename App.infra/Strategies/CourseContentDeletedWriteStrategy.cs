using App.Application.Common.Events;
using App.domain.ReadModels;
using App.domain.entity;
using App.infra.Persistence;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;

namespace App.infra.Strategies;

public class CourseContentDeletedWriteStrategy : IDomainEventWriteStrategy
{
    private readonly AppReadDbContext _readContext;

    public CourseContentDeletedWriteStrategy(AppReadDbContext readContext)
    {
        _readContext = readContext;
    }

    public async Task ProcessAsync(DomainEvent domainEvent)
    {
        var data = JsonSerializer.Deserialize<CourseContentDeletedData>(domainEvent.Data);
        if (data == null) return;

        var contentRead = await _readContext.CourseContentsRead.FindAsync(data.Content_id);
        if (contentRead == null) return;

        _readContext.CourseContentsRead.Remove(contentRead);
        await _readContext.SaveChangesAsync();
    }

    private record CourseContentDeletedData(string Content_id);
}
