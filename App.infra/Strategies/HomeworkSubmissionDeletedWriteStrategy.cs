using App.Application.Common.Events;
using App.domain.ReadModels;
using App.domain.entity;
using App.infra.Persistence;
using App.infra.Caching;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;

namespace App.infra.Strategies;

public class HomeworkSubmissionDeletedWriteStrategy : IDomainEventWriteStrategy
{
    private readonly AppReadDbContext _readContext;
    private readonly ICacheInvalidationService _cacheInvalidation;

    public HomeworkSubmissionDeletedWriteStrategy(AppReadDbContext readContext, ICacheInvalidationService cacheInvalidation)
    {
        _readContext = readContext;
        _cacheInvalidation = cacheInvalidation;
    }

    public async Task ProcessAsync(DomainEvent domainEvent)
    {
        var data = JsonSerializer.Deserialize<HomeworkSubmissionDeletedData>(domainEvent.Data);
        if (data == null) return;

        var submissionRead = await _readContext.HomeworkSubmissionsRead.FindAsync(data.Submission_id);
        if (submissionRead == null) return;

        _readContext.HomeworkSubmissionsRead.Remove(submissionRead);
        await _readContext.SaveChangesAsync();
        await _cacheInvalidation.InvalidateByEntityAsync("HomeworkSubmissionRead", data.Submission_id);
    }

    private record HomeworkSubmissionDeletedData(string Submission_id);
}
