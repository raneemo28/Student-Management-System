using App.LoggingMicroservice.Data;
using App.LoggingMicroservice.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;
using System.Text.Json;

namespace App.LoggingMicroservice.Consumers;

public class LogConsumer : BackgroundService
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<LogConsumer> _logger;
    private readonly string _connectionString;
    private readonly string _queueName;
    private IConnection? _connection;
    private IChannel? _channel;

    public LogConsumer(IServiceProvider serviceProvider, ILogger<LogConsumer> logger, IConfiguration configuration)
    {
        _serviceProvider = serviceProvider;
        _logger = logger;
        _connectionString = configuration.GetConnectionString("RabbitMQ") ?? "amqp://guest:guest@rabbitmq:5672";
        _queueName = configuration["RabbitMQ:QueueName"] ?? "logs";
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        await InitializeRabbitMQAsync(stoppingToken);

        if (_channel == null)
        {
            _logger.LogError("Failed to initialize RabbitMQ connection");
            return;
        }

        var consumer = new AsyncEventingBasicConsumer(_channel);
        consumer.ReceivedAsync += async (sender, ea) =>
        {
            await ProcessMessageAsync(ea, stoppingToken);
        };

        await _channel.BasicConsumeAsync(_queueName, autoAck: false, consumer, cancellationToken: stoppingToken);
        
        _logger.LogInformation("Log consumer started, listening on queue: {QueueName}", _queueName);
    }

    private async Task InitializeRabbitMQAsync(CancellationToken cancellationToken)
    {
        var retryCount = 0;
        const int maxRetries = 10;
        const int delayMs = 5000;

        while (retryCount < maxRetries && !cancellationToken.IsCancellationRequested)
        {
            try
            {
                var factory = new ConnectionFactory
                {
                    Uri = new Uri(_connectionString),
                    AutomaticRecoveryEnabled = true,
                    NetworkRecoveryInterval = TimeSpan.FromSeconds(10)
                };

                _connection = await factory.CreateConnectionAsync(cancellationToken);
                _channel = await _connection.CreateChannelAsync(cancellationToken: cancellationToken);

                await _channel.QueueDeclareAsync(
                    queue: _queueName,
                    durable: true,
                    exclusive: false,
                    autoDelete: false,
                    arguments: null,
                    cancellationToken: cancellationToken);

                _logger.LogInformation("Connected to RabbitMQ successfully");
                return;
            }
            catch (Exception ex)
            {
                retryCount++;
                _logger.LogWarning(ex, "Failed to connect to RabbitMQ (attempt {RetryCount}/{MaxRetries}). Retrying in {Delay}ms...", 
                    retryCount, maxRetries, delayMs);
                
                if (retryCount >= maxRetries)
                {
                    _logger.LogError(ex, "Failed to connect to RabbitMQ after {MaxRetries} attempts", maxRetries);
                    throw;
                }

                await Task.Delay(delayMs, cancellationToken);
            }
        }
    }

    private async Task ProcessMessageAsync(BasicDeliverEventArgs ea, CancellationToken stoppingToken)
    {
        try
        {
            var body = ea.Body.ToArray();
            var message = Encoding.UTF8.GetString(body);
            var logMessage = JsonSerializer.Deserialize<LogMessage>(message);

            if (logMessage == null)
            {
                _logger.LogWarning("Received null log message, acknowledging");
                await _channel!.BasicAckAsync(ea.DeliveryTag, multiple: false, cancellationToken: stoppingToken);
                return;
            }

            using var scope = _serviceProvider.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<LogsDbContext>();

            var logEntry = new LogEntry
            {
                Level = logMessage.Level,
                Message = logMessage.Message,
                Exception = logMessage.Exception,
                Source = logMessage.Source,
                CorrelationId = logMessage.CorrelationId,
                UserId = logMessage.UserId,
                RequestPath = logMessage.RequestPath,
                RequestMethod = logMessage.RequestMethod,
                Properties = logMessage.Properties,
                Timestamp = logMessage.Timestamp,
                MachineName = logMessage.MachineName,
                ThreadId = logMessage.ThreadId
            };

            await dbContext.LogEntries.AddAsync(logEntry, stoppingToken);
            await dbContext.SaveChangesAsync(stoppingToken);

            await _channel!.BasicAckAsync(ea.DeliveryTag, multiple: false, cancellationToken: stoppingToken);
            
            _logger.LogDebug("Log entry saved: {Level} - {Message}", logEntry.Level, logEntry.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error processing log message");
            
            try
            {
                await _channel!.BasicNackAsync(ea.DeliveryTag, multiple: false, requeue: true, cancellationToken: stoppingToken);
            }
            catch (Exception nackEx)
            {
                _logger.LogError(nackEx, "Failed to nack message");
            }
        }
    }

    public override async Task StopAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation("Stopping log consumer...");
        
        if (_channel != null)
        {
            await _channel.CloseAsync(cancellationToken);
        }
        
        if (_connection != null)
        {
            await _connection.CloseAsync(cancellationToken);
        }

        await base.StopAsync(cancellationToken);
    }
}