using App.Application.Common.Events;
using App.domain.ReadModels;
using App.domain.entity;
using App.infra.Persistence;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;

namespace App.infra.Strategies;

public class InstructorCreatedWriteStrategy : IDomainEventWriteStrategy
{
    private readonly AppReadDbContext _readContext;

    public InstructorCreatedWriteStrategy(AppReadDbContext readContext)
    {
        _readContext = readContext;
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
    }

    private record InstructorCreatedData(string Instructor_id, string FirstName, string LastName);
}
