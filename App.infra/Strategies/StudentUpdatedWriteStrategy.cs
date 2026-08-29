using App.Application.Common.Events;
using App.domain.ReadModels;
using App.domain.entity;
using App.infra.Persistence;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;

namespace App.infra.Strategies;

public class StudentUpdatedWriteStrategy : IDomainEventWriteStrategy
{
    private readonly AppReadDbContext _readContext;

    public StudentUpdatedWriteStrategy(AppReadDbContext readContext)
    {
        _readContext = readContext;
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
    }

    private record StudentUpdatedData(string Student_id, string FirstName, string LastName);
}
