using App.Application.Common.Events;
using App.domain.ReadModels;
using App.domain.entity;
using App.infra.Persistence;
using App.infra.Caching;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;

namespace App.infra.Strategies;

public class CourseContentUpdatedWriteStrategy : IDomainEventWriteStrategy
{
    private readonly AppReadDbContext _readContext;
    private readonly ICacheInvalidationService _cacheInvalidation;

    public CourseContentUpdatedWriteStrategy(AppReadDbContext readContext, ICacheInvalidationService cacheInvalidation)
    {
        _readContext = readContext;
        _cacheInvalidation = cacheInvalidation;
    }

    public async Task ProcessAsync(DomainEvent domainEvent)
    {
        var data = JsonSerializer.Deserialize<CourseContentUpdatedData>(domainEvent.Data);
        if (data == null) return;

        var contentRead = await _readContext.CourseContentsRead.FindAsync(data.Content_id);
        if (contentRead == null) return;

        contentRead.Title = data.Title;
        contentRead.Description = data.Description;
        contentRead.ContentType = data.ContentType;
        contentRead.FileName = data.FileName;
        contentRead.FileUrl = data.FileUrl;
        contentRead.FileSize = data.FileSize;
        contentRead.CreatedAt = data.UpdatedAt;
        await _readContext.SaveChangesAsync();
        await _cacheInvalidation.InvalidateByEntityAsync("CourseContentRead", data.Content_id);
    }

    private record CourseContentUpdatedData(string Content_id, string Title, string Description, string ContentType, string FileName, string FileUrl, long FileSize, DateTime UpdatedAt);
}
