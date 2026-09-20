using App.Application.Common.Events;
using App.domain.ReadModels;
using App.domain.entity;
using App.infra.Persistence;
using App.infra.Caching;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;

namespace App.infra.Strategies;

public class HomeworkSubmissionCreatedWriteStrategy : IDomainEventWriteStrategy
{
    private readonly AppReadDbContext _readContext;
    private readonly ICacheInvalidationService _cacheInvalidation;

    public HomeworkSubmissionCreatedWriteStrategy(AppReadDbContext readContext, ICacheInvalidationService cacheInvalidation)
    {
        _readContext = readContext;
        _cacheInvalidation = cacheInvalidation;
    }

    public async Task ProcessAsync(DomainEvent domainEvent)
    {
        var data = JsonSerializer.Deserialize<HomeworkSubmissionCreatedData>(domainEvent.Data);
        if (data == null) return;

        var submissionRead = new HomeworkSubmissionRead
        {
            Submission_id = data.Submission_id,
            Homework_id = data.Homework_id,
            Student_id = data.Student_id,
            SolutionFileName = data.SolutionFileName,
            SolutionFileUrl = data.SolutionFileUrl,
            SubmittedAt = data.SubmittedAt,
            Mark = data.Mark
        };

        _readContext.HomeworkSubmissionsRead.Add(submissionRead);
        await _readContext.SaveChangesAsync();
        await _cacheInvalidation.InvalidateByEntityAsync("HomeworkSubmissionRead", data.Submission_id);
    }

    private record HomeworkSubmissionCreatedData(string Submission_id, string Homework_id, string Student_id, string SolutionFileName, string SolutionFileUrl, DateTime SubmittedAt, double? Mark);
}
