using App.Application.Common.Events;
using App.domain.ReadModels;
using App.domain.entity;
using App.infra.Persistence;
using App.infra.Caching;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;

namespace App.infra.Strategies;

public class InstructorUpdatedWriteStrategy : IDomainEventWriteStrategy
{
    private readonly AppReadDbContext _readContext;
    private readonly ICacheInvalidationService _cacheInvalidation;

    public InstructorUpdatedWriteStrategy(AppReadDbContext readContext, ICacheInvalidationService cacheInvalidation)
    {
        _readContext = readContext;
        _cacheInvalidation = cacheInvalidation;
    }

    public async Task ProcessAsync(DomainEvent domainEvent)
    {
        var data = JsonSerializer.Deserialize<InstructorUpdatedData>(domainEvent.Data);
        if (data == null) return;

        var instructorRead = await _readContext.InstructorsRead.FindAsync(data.Instructor_id);
        if (instructorRead == null) return;

        instructorRead.FirstName = data.FirstName;
        instructorRead.LastName = data.LastName;
        await _readContext.SaveChangesAsync();
        await _cacheInvalidation.InvalidateByEntityAsync("InstructorRead", data.Instructor_id);
    }

    private record InstructorUpdatedData(string Instructor_id, string FirstName, string LastName);
}
