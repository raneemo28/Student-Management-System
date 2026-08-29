using App.Application.Common.Events;
using App.domain.ReadModels;
using App.domain.entity;
using App.infra.Persistence;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;

namespace App.infra.Strategies;

public class StudentDeletedWriteStrategy : IDomainEventWriteStrategy
{
    private readonly AppReadDbContext _readContext;

    public StudentDeletedWriteStrategy(AppReadDbContext readContext)
    {
        _readContext = readContext;
    }

    public async Task ProcessAsync(DomainEvent domainEvent)
    {
        var data = JsonSerializer.Deserialize<StudentDeletedData>(domainEvent.Data);
        if (data == null) return;

        var studentRead = await _readContext.StudentsRead.FindAsync(data.Student_id);
        if (studentRead == null) return;

        _readContext.StudentsRead.Remove(studentRead);
        await _readContext.SaveChangesAsync();
    }

    private record StudentDeletedData(string Student_id);
}
