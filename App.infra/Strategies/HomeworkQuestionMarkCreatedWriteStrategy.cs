using App.Application.Common.Events;
using App.domain.ReadModels;
using App.domain.entity;
using App.infra.Persistence;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;

namespace App.infra.Strategies;

public class HomeworkQuestionMarkCreatedWriteStrategy : IDomainEventWriteStrategy
{
    private readonly AppReadDbContext _readContext;

    public HomeworkQuestionMarkCreatedWriteStrategy(AppReadDbContext readContext)
    {
        _readContext = readContext;
    }

    public async Task ProcessAsync(DomainEvent domainEvent)
    {
        var data = JsonSerializer.Deserialize<HomeworkQuestionMarkCreatedData>(domainEvent.Data);
        if (data == null) return;

        var questionMarkRead = new HomeworkQuestionMarkRead
        {
            QuestionMark_id = data.QuestionMark_id,
            Submission_id = data.Submission_id,
            QuestionNumber = data.QuestionNumber,
            QuestionDescription = data.QuestionDescription,
            MaxMarks = data.MaxMarks,
            ObtainedMarks = data.ObtainedMarks
        };

        _readContext.HomeworkQuestionMarksRead.Add(questionMarkRead);
        await _readContext.SaveChangesAsync();
    }

    private record HomeworkQuestionMarkCreatedData(string QuestionMark_id, string Submission_id, int QuestionNumber, string QuestionDescription, double MaxMarks, double? ObtainedMarks);
}
