using App.Application.Common.Events;
using App.domain.ReadModels;
using App.domain.entity;
using App.infra.Persistence;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;

namespace App.infra.Strategies;

public class HomeworkDeletedWriteStrategy : IDomainEventWriteStrategy
{
    private readonly AppReadDbContext _readContext;

    public HomeworkDeletedWriteStrategy(AppReadDbContext readContext)
    {
        _readContext = readContext;
    }

    public async Task ProcessAsync(DomainEvent domainEvent)
    {
        var data = JsonSerializer.Deserialize<HomeworkDeletedData>(domainEvent.Data);
        if (data == null) return;

        var homeworkRead = await _readContext.HomeworksRead.FindAsync(data.Homework_id);
        if (homeworkRead == null) return;

        _readContext.HomeworksRead.Remove(homeworkRead);
        await _readContext.SaveChangesAsync();
    }

    private record HomeworkDeletedData(string Homework_id);
}
