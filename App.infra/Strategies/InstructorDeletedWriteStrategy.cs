using App.Application.Common.Events;
using App.domain.ReadModels;
using App.domain.entity;
using App.infra.Persistence;
using App.infra.Caching;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;

namespace App.infra.Strategies;

public class InstructorDeletedWriteStrategy : IDomainEventWriteStrategy
{
    private readonly AppReadDbContext _readContext;
    private readonly ICacheInvalidationService _cacheInvalidation;

    public InstructorDeletedWriteStrategy(AppReadDbContext readContext, ICacheInvalidationService cacheInvalidation)
    {
        _readContext = readContext;
        _cacheInvalidation = cacheInvalidation;
    }

    public async Task ProcessAsync(DomainEvent domainEvent)
    {
        var data = JsonSerializer.Deserialize<InstructorDeletedData>(domainEvent.Data);
        if (data == null) return;

        var instructorRead = await _readContext.InstructorsRead.FindAsync(data.Instructor_id);
        if (instructorRead == null) return;

        _readContext.InstructorsRead.Remove(instructorRead);
        await _readContext.SaveChangesAsync();
        await _cacheInvalidation.InvalidateByEntityAsync("InstructorRead", data.Instructor_id);
    }

    private record InstructorDeletedData(string Instructor_id);
}
