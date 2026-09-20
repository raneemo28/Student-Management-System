using App.Application.Common.Events;
using App.domain.ReadModels;
using App.domain.entity;
using App.infra.Persistence;
using App.infra.Caching;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;

namespace App.infra.Strategies;

public class HomeworkCreatedWriteStrategy : IDomainEventWriteStrategy
{
    private readonly AppReadDbContext _readContext;
    private readonly ICacheInvalidationService _cacheInvalidation;

    public HomeworkCreatedWriteStrategy(AppReadDbContext readContext, ICacheInvalidationService cacheInvalidation)
    {
        _readContext = readContext;
        _cacheInvalidation = cacheInvalidation;
    }

    public async Task ProcessAsync(DomainEvent domainEvent)
    {
        var data = JsonSerializer.Deserialize<HomeworkCreatedData>(domainEvent.Data);
        if (data == null) return;

        var homeworkRead = new HomeworkRead
        {
            Homework_id = data.Homework_id,
            Course_id = data.Course_id,
            Title = data.Title,
            Description = data.Description,
            QuestionFileName = data.QuestionFileName,
            QuestionFileUrl = data.QuestionFileUrl,
            DueDate = data.DueDate,
            TotalMarks = data.TotalMarks,
            CreatedAt = data.CreatedAt
        };

        _readContext.HomeworksRead.Add(homeworkRead);
        await _readContext.SaveChangesAsync();
        await _cacheInvalidation.InvalidateByEntityAsync("HomeworkRead", data.Homework_id);
    }

    private record HomeworkCreatedData(string Homework_id, string Course_id, string Title, string Description, string QuestionFileName, string QuestionFileUrl, DateTime DueDate, int TotalMarks, DateTime CreatedAt);
}
