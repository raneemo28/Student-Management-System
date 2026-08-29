using App.Application.Common.Events;
using App.domain.ReadModels;
using App.domain.entity;
using App.infra.Persistence;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;

namespace App.infra.Strategies;

public class HomeworkSubmissionDeletedWriteStrategy : IDomainEventWriteStrategy
{
    private readonly AppReadDbContext _readContext;

    public HomeworkSubmissionDeletedWriteStrategy(AppReadDbContext readContext)
    {
        _readContext = readContext;
    }

    public async Task ProcessAsync(DomainEvent domainEvent)
    {
        var data = JsonSerializer.Deserialize<HomeworkSubmissionDeletedData>(domainEvent.Data);
        if (data == null) return;

        var submissionRead = await _readContext.HomeworkSubmissionsRead.FindAsync(data.Submission_id);
        if (submissionRead == null) return;

        _readContext.HomeworkSubmissionsRead.Remove(submissionRead);
        await _readContext.SaveChangesAsync();
    }

    private record HomeworkSubmissionDeletedData(string Submission_id);
}
