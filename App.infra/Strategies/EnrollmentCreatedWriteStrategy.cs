using App.Application.Common.Events;
using App.domain.ReadModels;
using App.domain.entity;
using App.infra.Persistence;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;

namespace App.infra.Strategies;

public class EnrollmentCreatedWriteStrategy : IDomainEventWriteStrategy
{
    private readonly AppReadDbContext _readContext;

    public EnrollmentCreatedWriteStrategy(AppReadDbContext readContext)
    {
        _readContext = readContext;
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
    }

    private record EnrollmentCreatedData(string Student_id, string Course_id, DateTime EnrolledAt);
}
