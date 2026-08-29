using App.Application.Common.Events;
using App.domain.ReadModels;
using App.domain.entity;
using App.infra.Persistence;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;

namespace App.infra.Strategies;

public class HomeworkQuestionMarkDeletedWriteStrategy : IDomainEventWriteStrategy
{
    private readonly AppReadDbContext _readContext;

    public HomeworkQuestionMarkDeletedWriteStrategy(AppReadDbContext readContext)
    {
        _readContext = readContext;
    }

    public async Task ProcessAsync(DomainEvent domainEvent)
    {
        var data = JsonSerializer.Deserialize<HomeworkQuestionMarkDeletedData>(domainEvent.Data);
        if (data == null) return;

        var questionMarkRead = await _readContext.HomeworkQuestionMarksRead.FindAsync(data.QuestionMark_id);
        if (questionMarkRead == null) return;

        _readContext.HomeworkQuestionMarksRead.Remove(questionMarkRead);
        await _readContext.SaveChangesAsync();
    }

    private record HomeworkQuestionMarkDeletedData(string QuestionMark_id);
}
