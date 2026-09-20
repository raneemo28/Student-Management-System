using App.Application.Common.Events;
using App.domain.ReadModels;
using App.domain.entity;
using App.infra.Persistence;
using App.infra.Caching;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;

namespace App.infra.Strategies;

public class InstructorCreatedWriteStrategy : IDomainEventWriteStrategy
{
    private readonly AppReadDbContext _readContext;
    private readonly ICacheInvalidationService _cacheInvalidation;

    public InstructorCreatedWriteStrategy(AppReadDbContext readContext, ICacheInvalidationService cacheInvalidation)
    {
        _readContext = readContext;
        _cacheInvalidation = cacheInvalidation;
    }

    public async Task ProcessAsync(DomainEvent domainEvent)
    {
        var data = JsonSerializer.Deserialize<InstructorCreatedData>(domainEvent.Data);
        if (data == null) return;

        var instructorRead = new InstructorRead
        {
            Instructor_id = data.Instructor_id,
            FirstName = data.FirstName,
            LastName = data.LastName
        };

        _readContext.InstructorsRead.Add(instructorRead);
        await _readContext.SaveChangesAsync();
        await _cacheInvalidation.InvalidateByEntityAsync("InstructorRead", data.Instructor_id);
    }

    private record InstructorCreatedData(string Instructor_id, string FirstName, string LastName);
}
