using App.Application.Common.Events;
using App.domain.ReadModels;
using App.domain.entity;
using App.infra.Persistence;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;

namespace App.infra.Strategies;

public class InstructorUpdatedWriteStrategy : IDomainEventWriteStrategy
{
    private readonly AppReadDbContext _readContext;

    public InstructorUpdatedWriteStrategy(AppReadDbContext readContext)
    {
        _readContext = readContext;
    }

    public async Task ProcessAsync(DomainEvent domainEvent)
    {
        var data = JsonSerializer.Deserialize<InstructorUpdatedData>(domainEvent.Data);
        if (data == null) return;

        var instructorRead = await _readContext.InstructorsRead.FindAsync(data.Instructor_id);
        if (instructorRead == null) return;

        instructorRead.FirstName = data.FirstName;
        instructorRead.LastName = data.LastName;
        await _readContext.SaveChangesAsync();
    }

    private record InstructorUpdatedData(string Instructor_id, string FirstName, string LastName);
}
