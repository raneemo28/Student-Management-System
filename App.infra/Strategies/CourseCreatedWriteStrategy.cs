using App.Application.Common.Events;
using App.domain.ReadModels;
using App.domain.entity;
using App.infra.Persistence;
using App.infra.Caching;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;

namespace App.infra.Strategies;

public class CourseCreatedWriteStrategy : IDomainEventWriteStrategy
{
    private readonly AppReadDbContext _readContext;
    private readonly ICacheInvalidationService _cacheInvalidation;

    public CourseCreatedWriteStrategy(AppReadDbContext readContext, ICacheInvalidationService cacheInvalidation)
    {
        _readContext = readContext;
        _cacheInvalidation = cacheInvalidation;
    }

    public async Task ProcessAsync(DomainEvent domainEvent)
    {
        var data = JsonSerializer.Deserialize<CourseCreatedData>(domainEvent.Data);
        if (data == null) return;

        var courseRead = new CourseRead
        {
            Course_id = data.Course_id,
            Course_name = data.Course_name,
            Instructor_id = data.Instructor_id
        };

        _readContext.CoursesRead.Add(courseRead);
        await _readContext.SaveChangesAsync();
        await _cacheInvalidation.InvalidateByEntityAsync("CourseRead", data.Course_id);
    }

    private record CourseCreatedData(string Course_id, string Course_name, string? Instructor_id);
}

