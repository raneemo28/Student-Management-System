using App.Application.Common.Events;
using App.domain.ReadModels;
using App.domain.entity;
using App.infra.Persistence;
using App.infra.Caching;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;

namespace App.infra.Strategies;

public class HomeworkSolutionDeletedWriteStrategy : IDomainEventWriteStrategy
{
    private readonly AppReadDbContext _readContext;
    private readonly ICacheInvalidationService _cacheInvalidation;

    public HomeworkSolutionDeletedWriteStrategy(AppReadDbContext readContext, ICacheInvalidationService cacheInvalidation)
    {
        _readContext = readContext;
        _cacheInvalidation = cacheInvalidation;
    }

    public async Task ProcessAsync(DomainEvent domainEvent)
    {
        var data = JsonSerializer.Deserialize<HomeworkSolutionDeletedData>(domainEvent.Data);
        if (data == null) return;

        var solutionRead = await _readContext.HomeworkSolutionsRead.FindAsync(data.Solution_id);
        if (solutionRead == null) return;

        _readContext.HomeworkSolutionsRead.Remove(solutionRead);
        await _readContext.SaveChangesAsync();
        await _cacheInvalidation.InvalidateByEntityAsync("HomeworkSolutionRead", data.Solution_id);
    }

    private record HomeworkSolutionDeletedData(string Solution_id);
}
