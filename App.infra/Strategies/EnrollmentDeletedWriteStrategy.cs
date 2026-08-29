using App.Application.Common.Events;
using App.domain.ReadModels;
using App.domain.entity;
using App.infra.Persistence;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;

namespace App.infra.Strategies;

public class EnrollmentDeletedWriteStrategy : IDomainEventWriteStrategy
{
    private readonly AppReadDbContext _readContext;

    public EnrollmentDeletedWriteStrategy(AppReadDbContext readContext)
    {
        _readContext = readContext;
    }

    public async Task ProcessAsync(DomainEvent domainEvent)
    {
        var data = JsonSerializer.Deserialize<EnrollmentDeletedData>(domainEvent.Data);
        if (data == null) return;

        var enrollmentRead = await _readContext.EnrollmentsRead
            .FirstOrDefaultAsync(cs => cs.Student_id == data.Student_id && cs.Course_id == data.Course_id);
        
        if (enrollmentRead == null) return;

        _readContext.EnrollmentsRead.Remove(enrollmentRead);
        await _readContext.SaveChangesAsync();
    }

    private record EnrollmentDeletedData(string Student_id, string Course_id);
}
