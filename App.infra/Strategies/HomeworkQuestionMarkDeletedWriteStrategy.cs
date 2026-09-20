using App.Application.Common.Events;
using App.domain.ReadModels;
using App.domain.entity;
using App.infra.Persistence;
using App.infra.Caching;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;

namespace App.infra.Strategies;

public class HomeworkQuestionMarkDeletedWriteStrategy : IDomainEventWriteStrategy
{
    private readonly AppReadDbContext _readContext;
    private readonly ICacheInvalidationService _cacheInvalidation;

    public HomeworkQuestionMarkDeletedWriteStrategy(AppReadDbContext readContext, ICacheInvalidationService cacheInvalidation)
    {
        _readContext = readContext;
        _cacheInvalidation = cacheInvalidation;
    }

    public async Task ProcessAsync(DomainEvent domainEvent)
    {
        var data = JsonSerializer.Deserialize<HomeworkQuestionMarkDeletedData>(domainEvent.Data);
        if (data == null) return;

        var questionMarkRead = await _readContext.HomeworkQuestionMarksRead.FindAsync(data.QuestionMark_id);
        if (questionMarkRead == null) return;

        _readContext.HomeworkQuestionMarksRead.Remove(questionMarkRead);
        await _readContext.SaveChangesAsync();
        await _cacheInvalidation.InvalidateByEntityAsync("HomeworkQuestionMarkRead", data.QuestionMark_id);
    }

    private record HomeworkQuestionMarkDeletedData(string QuestionMark_id);
}
