using App.Application.Common.Events;
using App.domain.ReadModels;
using App.domain.entity;
using App.infra.Persistence;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;

namespace App.infra.Strategies;

public class HomeworkUpdatedWriteStrategy : IDomainEventWriteStrategy
{
    private readonly AppReadDbContext _readContext;

    public HomeworkUpdatedWriteStrategy(AppReadDbContext readContext)
    {
        _readContext = readContext;
    }

    public async Task ProcessAsync(DomainEvent domainEvent)
    {
        var data = JsonSerializer.Deserialize<HomeworkUpdatedData>(domainEvent.Data);
        if (data == null) return;

        var homeworkRead = await _readContext.HomeworksRead.FindAsync(data.Homework_id);
        if (homeworkRead == null) return;

        homeworkRead.Title = data.Title;
        homeworkRead.Description = data.Description;
        homeworkRead.DueDate = data.DueDate;
        homeworkRead.TotalMarks = data.TotalMarks;
        homeworkRead.QuestionFileName = data.QuestionFileName;
        homeworkRead.QuestionFileUrl = data.QuestionFileUrl;
        await _readContext.SaveChangesAsync();
    }

    private record HomeworkUpdatedData(string Homework_id, string Title, string Description, DateTime DueDate, int TotalMarks, string QuestionFileName, string QuestionFileUrl);
}
