using App.Application.Common.Events;
using App.domain.ReadModels;
using App.domain.entity;
using App.infra.Persistence;
using App.infra.Caching;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;

namespace App.infra.Strategies;

public class StudentCreatedWriteStrategy : IDomainEventWriteStrategy
{
    private readonly AppReadDbContext _readContext;
    private readonly ICacheInvalidationService _cacheInvalidation;

    public StudentCreatedWriteStrategy(AppReadDbContext readContext, ICacheInvalidationService cacheInvalidation)
    {
        _readContext = readContext;
        _cacheInvalidation = cacheInvalidation;
    }

    public async Task ProcessAsync(DomainEvent domainEvent)
    {
        var data = JsonSerializer.Deserialize<StudentCreatedData>(domainEvent.Data);
        if (data == null) return;

        var studentRead = new StudentRead
        {
            Student_id = data.Student_id,
            FirstName = data.FirstName,
            LastName = data.LastName
        };

        _readContext.StudentsRead.Add(studentRead);
        await _readContext.SaveChangesAsync();
        await _cacheInvalidation.InvalidateByEntityAsync("StudentRead", data.Student_id);
    }

    private record StudentCreatedData(string Student_id, string FirstName, string LastName);
}
