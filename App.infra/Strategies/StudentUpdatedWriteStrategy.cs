using App.Application.Common.Events;
using App.domain.ReadModels;
using App.domain.entity;
using App.infra.Persistence;
using App.infra.Caching;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;

namespace App.infra.Strategies;

public class StudentUpdatedWriteStrategy : IDomainEventWriteStrategy
{
    private readonly AppReadDbContext _readContext;
    private readonly ICacheInvalidationService _cacheInvalidation;

    public StudentUpdatedWriteStrategy(AppReadDbContext readContext, ICacheInvalidationService cacheInvalidation)
    {
        _readContext = readContext;
        _cacheInvalidation = cacheInvalidation;
    }

    public async Task ProcessAsync(DomainEvent domainEvent)
    {
        var data = JsonSerializer.Deserialize<StudentUpdatedData>(domainEvent.Data);
        if (data == null) return;

        var studentRead = await _readContext.StudentsRead.FindAsync(data.Student_id);
        if (studentRead == null) return;

        studentRead.FirstName = data.FirstName;
        studentRead.LastName = data.LastName;
        await _readContext.SaveChangesAsync();
        await _cacheInvalidation.InvalidateByEntityAsync("StudentRead", data.Student_id);
    }

    private record StudentUpdatedData(string Student_id, string FirstName, string LastName);
}
