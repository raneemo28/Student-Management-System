using App.Application.Common.Events;
using App.domain.ReadModels;
using App.domain.entity;
using App.infra.Persistence;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;

namespace App.infra.Strategies;

public class HomeworkSubmissionCreatedWriteStrategy : IDomainEventWriteStrategy
{
    private readonly AppReadDbContext _readContext;

    public HomeworkSubmissionCreatedWriteStrategy(AppReadDbContext readContext)
    {
        _readContext = readContext;
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
    }

    private record HomeworkSubmissionCreatedData(string Submission_id, string Homework_id, string Student_id, string SolutionFileName, string SolutionFileUrl, DateTime SubmittedAt, double? Mark);
}
