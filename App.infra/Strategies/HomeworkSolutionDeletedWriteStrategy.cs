using App.Application.Common.Events;
using App.domain.ReadModels;
using App.domain.entity;
using App.infra.Persistence;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;

namespace App.infra.Strategies;

public class HomeworkSolutionDeletedWriteStrategy : IDomainEventWriteStrategy
{
    private readonly AppReadDbContext _readContext;

    public HomeworkSolutionDeletedWriteStrategy(AppReadDbContext readContext)
    {
        _readContext = readContext;
    }

    public async Task ProcessAsync(DomainEvent domainEvent)
    {
        var data = JsonSerializer.Deserialize<HomeworkSolutionDeletedData>(domainEvent.Data);
        if (data == null) return;

        var solutionRead = await _readContext.HomeworkSolutionsRead.FindAsync(data.Solution_id);
        if (solutionRead == null) return;

        _readContext.HomeworkSolutionsRead.Remove(solutionRead);
        await _readContext.SaveChangesAsync();
    }

    private record HomeworkSolutionDeletedData(string Solution_id);
}
