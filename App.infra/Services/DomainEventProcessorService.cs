using App.Application.Common.Events;
using App.domain.entity;
using App.infra.Persistence;
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

    public DomainEventProcessorService(IServiceScopeFactory scopeFactory, ILogger<DomainEventProcessorService> logger)
    {
        _scopeFactory = scopeFactory;
        _logger = logger;
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
}
