using App.Application.Common.Events;
using App.domain.ReadModels;
using App.domain.entity;
using App.infra.Persistence;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;

namespace App.infra.Strategies;

public class HomeworkSolutionCreatedWriteStrategy : IDomainEventWriteStrategy
{
    private readonly AppReadDbContext _readContext;

    public HomeworkSolutionCreatedWriteStrategy(AppReadDbContext readContext)
    {
        _readContext = readContext;
    }

    public async Task ProcessAsync(DomainEvent domainEvent)
    {
        var data = JsonSerializer.Deserialize<HomeworkSolutionCreatedData>(domainEvent.Data);
        if (data == null) return;

        var solutionRead = new HomeworkSolutionRead
        {
            Solution_id = data.Solution_id,
            Homework_id = data.Homework_id,
            SolutionFileName = data.SolutionFileName,
            SolutionFileUrl = data.SolutionFileUrl,
            UploadedAt = data.UploadedAt,
            Notes = data.Notes
        };

        _readContext.HomeworkSolutionsRead.Add(solutionRead);
        await _readContext.SaveChangesAsync();
    }

    private record HomeworkSolutionCreatedData(string Solution_id, string Homework_id, string SolutionFileName, string SolutionFileUrl, DateTime UploadedAt, string? Notes);
}
