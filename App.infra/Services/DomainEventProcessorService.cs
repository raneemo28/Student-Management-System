using App.Application.Common.Events;
using App.domain.entity;
using App.infra.Persistence;
using App.infra.Caching;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using App.infra.Strategies;

namespace App.infra.Services;

public class DomainEventProcessorService : BackgroundService
{
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly ILogger<DomainEventProcessorService> _logger;
    private readonly ICacheInvalidationService _cacheInvalidation;

    public DomainEventProcessorService(
        IServiceScopeFactory scopeFactory,
        ILogger<DomainEventProcessorService> logger,
        ICacheInvalidationService cacheInvalidation)
    {
        _scopeFactory = scopeFactory;
        _logger = logger;
        _cacheInvalidation = cacheInvalidation;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("Domain Event Processor Service started.");

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                using var scope = _scopeFactory.CreateScope();
                var writeContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
                var readContext = scope.ServiceProvider.GetRequiredService<AppReadDbContext>();
                var strategyFactory = scope.ServiceProvider.GetRequiredService<DomainEventWriteStrategyFactory>();

                var unprocessedEvents = await writeContext.DomainEvents
                    .Where(e => !e.IsProcessed)
                    .OrderBy(e => e.OccurredOn)
                    .Take(100)
                    .ToListAsync(stoppingToken);

                if (unprocessedEvents.Any())
                {
                    _logger.LogInformation("Processing {Count} domain events.", unprocessedEvents.Count);
                }

                foreach (var domainEvent in unprocessedEvents)
                {
                    try
                    {
                        if (strategyFactory.TryGetStrategy(domainEvent.EventType, out var strategy) && strategy != null)
                        {
                            await strategy.ProcessAsync(domainEvent);
                            domainEvent.IsProcessed = true;
                            domainEvent.ProcessedOn = DateTime.UtcNow;

                            var entityType = ExtractEntityTypeName(domainEvent.EventType);
                            if (entityType != null)
                            {
                                await _cacheInvalidation.InvalidateByEntityAsync(entityType, ExtractEntityId(domainEvent));
                            }
                        }
                        else
                        {
                            _logger.LogWarning("Unknown domain event type: {EventType}", domainEvent.EventType);
                        }
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "Error processing domain event {EventId} of type {EventType}", domainEvent.Id, domainEvent.EventType);
                    }
                }

                if (unprocessedEvents.Any())
                {
                    await writeContext.SaveChangesAsync(stoppingToken);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in Domain Event Processor loop.");
            }

            await Task.Delay(TimeSpan.FromSeconds(5), stoppingToken);
        }
    }

    private static string? ExtractEntityTypeName(string eventType)
    {
        if (eventType.Contains("Student")) return "StudentRead";
        if (eventType.Contains("Instructor")) return "InstructorRead";
        if (eventType.Contains("CourseCreated") || eventType.Contains("CourseUpdated") || eventType.Contains("CourseDeleted")) return "CourseRead";
        if (eventType.Contains("Enrollment")) return "CourseStudentRead";
        if (eventType.Contains("CourseContent")) return "CourseContentRead";
        if (eventType.Contains("Employee")) return "EmployeeRead";
        if (eventType.Contains("Advertisement")) return "AdvertisementRead";
        if (eventType.Contains("Homework") && !eventType.Contains("Submission") && !eventType.Contains("Solution") && !eventType.Contains("Question")) return "HomeworkRead";
        if (eventType.Contains("HomeworkSubmission")) return "HomeworkSubmissionRead";
        if (eventType.Contains("HomeworkSolution")) return "HomeworkSolutionRead";
        if (eventType.Contains("HomeworkQuestion")) return "HomeworkQuestionMarkRead";
        return null;
    }

    private static string ExtractEntityId(DomainEvent domainEvent)
    {
        var data = System.Text.Json.JsonSerializer.Deserialize<System.Text.Json.JsonElement>(domainEvent.Data);
        string id = data.TryGetProperty("Student_id", out var v1) ? v1.GetString() ?? ""
            : data.TryGetProperty("Instructor_id", out var v2) ? v2.GetString() ?? ""
            : data.TryGetProperty("Course_id", out var v3) ? v3.GetString() ?? ""
            : data.TryGetProperty("Content_id", out var v4) ? v4.GetString() ?? ""
            : data.TryGetProperty("Employee_id", out var v5) ? v5.GetString() ?? ""
            : data.TryGetProperty("Advertisement_id", out var v6) ? v6.GetString() ?? ""
            : data.TryGetProperty("Homework_id", out var v7) ? v7.GetString() ?? ""
            : data.TryGetProperty("Submission_id", out var v8) ? v8.GetString() ?? ""
            : data.TryGetProperty("Solution_id", out var v9) ? v9.GetString() ?? ""
            : data.TryGetProperty("QuestionMark_id", out var v10) ? v10.GetString() ?? ""
            : domainEvent.Id.ToString();
        return id;
    }
}