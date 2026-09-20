using App.Application.Common.Events;
using App.domain.ReadModels;
using App.domain.entity;
using App.infra.Persistence;
using App.infra.Caching;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;

namespace App.infra.Strategies;

public class CourseContentCreatedWriteStrategy : IDomainEventWriteStrategy
{
    private readonly AppReadDbContext _readContext;
    private readonly ICacheInvalidationService _cacheInvalidation;

    public CourseContentCreatedWriteStrategy(AppReadDbContext readContext, ICacheInvalidationService cacheInvalidation)
    {
        _readContext = readContext;
        _cacheInvalidation = cacheInvalidation;
    }

    public async Task ProcessAsync(DomainEvent domainEvent)
    {
        var data = JsonSerializer.Deserialize<CourseContentCreatedData>(domainEvent.Data);
        if (data == null) return;

        var contentRead = new CourseContentRead
        {
            Content_id = data.Content_id,
            Course_id = data.Course_id,
            Title = data.Title,
            Description = data.Description,
            ContentType = data.ContentType,
            FileName = data.FileName,
            FileUrl = data.FileUrl,
            FileSize = data.FileSize,
            CreatedAt = data.CreatedAt
        };

        _readContext.CourseContentsRead.Add(contentRead);
        await _readContext.SaveChangesAsync();
        await _cacheInvalidation.InvalidateByEntityAsync("CourseContentRead", data.Content_id);
    }

    private record CourseContentCreatedData(string Content_id, string Course_id, string Title, string Description, string ContentType, string FileName, string FileUrl, long FileSize, DateTime CreatedAt);
}
