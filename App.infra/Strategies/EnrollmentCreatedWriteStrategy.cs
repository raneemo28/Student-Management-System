using App.Application.Common.Events;
using App.domain.ReadModels;
using App.domain.entity;
using App.infra.Persistence;
using App.infra.Caching;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;

namespace App.infra.Strategies;

public class EnrollmentCreatedWriteStrategy : IDomainEventWriteStrategy
{
    private readonly AppReadDbContext _readContext;
    private readonly ICacheInvalidationService _cacheInvalidation;

    public EnrollmentCreatedWriteStrategy(AppReadDbContext readContext, ICacheInvalidationService cacheInvalidation)
    {
        _readContext = readContext;
        _cacheInvalidation = cacheInvalidation;
    }

    public async Task ProcessAsync(DomainEvent domainEvent)
    {
        var data = JsonSerializer.Deserialize<EnrollmentCreatedData>(domainEvent.Data);
        if (data == null) return;

        var enrollmentRead = new CourseStudentRead
        {
            Student_id = data.Student_id,
            Course_id = data.Course_id,
            EnrolledAt = data.EnrolledAt
        };

        _readContext.EnrollmentsRead.Add(enrollmentRead);
        await _readContext.SaveChangesAsync();
        await _cacheInvalidation.InvalidateByEntityAsync("CourseStudentRead", data.Student_id);
        await _cacheInvalidation.InvalidateByEntityAsync("CourseStudentRead", data.Course_id);
    }

    private record EnrollmentCreatedData(string Student_id, string Course_id, DateTime EnrolledAt);
}
