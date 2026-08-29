using App.Application.Common.Events;
using App.domain.ReadModels;
using App.domain.entity;
using App.infra.Persistence;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;

namespace App.infra.Strategies;

public class StudentCreatedWriteStrategy : IDomainEventWriteStrategy
{
    private readonly AppReadDbContext _readContext;

    public StudentCreatedWriteStrategy(AppReadDbContext readContext)
    {
        _readContext = readContext;
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
    }

    private record StudentCreatedData(string Student_id, string FirstName, string LastName);
}
