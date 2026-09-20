using App.Application.Common.Events;
using App.domain.ReadModels;
using App.domain.entity;
using App.infra.Persistence;
using App.infra.Caching;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;

namespace App.infra.Strategies;

public class CourseUpdatedWriteStrategy : IDomainEventWriteStrategy
{
    private readonly AppReadDbContext _readContext;
    private readonly ICacheInvalidationService _cacheInvalidation;

    public CourseUpdatedWriteStrategy(AppReadDbContext readContext, ICacheInvalidationService cacheInvalidation)
    {
        _readContext = readContext;
        _cacheInvalidation = cacheInvalidation;
    }

    public async Task ProcessAsync(DomainEvent domainEvent)
    {
        var data = JsonSerializer.Deserialize<CourseUpdatedData>(domainEvent.Data);
        if (data == null) return;

        var courseRead = await _readContext.CoursesRead.FindAsync(data.Course_id);
        if (courseRead == null) return;

        courseRead.Course_name = data.Course_name;
        courseRead.Instructor_id = data.Instructor_id;
        await _readContext.SaveChangesAsync();
        await _cacheInvalidation.InvalidateByEntityAsync("CourseRead", data.Course_id);
    }

    private record CourseUpdatedData(string Course_id, string Course_name, string? Instructor_id);
}
