using App.Application.Common.Events;
using App.domain.ReadModels;
using App.domain.entity;
using App.infra.Persistence;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;

namespace App.infra.Strategies;

public class CourseDeletedWriteStrategy : IDomainEventWriteStrategy
{
    private readonly AppReadDbContext _readContext;

    public CourseDeletedWriteStrategy(AppReadDbContext readContext)
    {
        _readContext = readContext;
    }

    public async Task ProcessAsync(DomainEvent domainEvent)
    {
        var data = JsonSerializer.Deserialize<CourseDeletedData>(domainEvent.Data);
        if (data == null) return;

        var courseRead = await _readContext.CoursesRead.FindAsync(data.Course_id);
        if (courseRead == null) return;

        _readContext.CoursesRead.Remove(courseRead);
        await _readContext.SaveChangesAsync();
    }

    private record CourseDeletedData(string Course_id);
}
