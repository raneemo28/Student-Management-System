using App.infra.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client;
using System.Text;
using System.Text.Json;

public interface ILogPublisher
{
    Task PublishLogAsync(LogMessage logMessage);
}

public class LogMessage
{
    public string Level { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
    public string? Exception { get; set; }
    public string? Source { get; set; }
    public string? CorrelationId { get; set; }
    public string? UserId { get; set; }
    public string? RequestPath { get; set; }
    public string? RequestMethod { get; set; }
    public Dictionary<string, object>? Properties { get; set; }
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;
    public string? MachineName { get; set; }
    public string? ThreadId { get; set; }
}

public class RabbitMqLogPublisher : ILogPublisher, IDisposable
{
    private readonly IConnection _connection;
    private readonly IChannel _channel;
    private readonly string _queueName;
    private readonly ILogger<RabbitMqLogPublisher> _logger;

    public RabbitMqLogPublisher(IConfiguration configuration, ILogger<RabbitMqLogPublisher> logger)
    {
        _logger = logger;
        _queueName = configuration["RabbitMQ:QueueName"] ?? "logs";
        
        var connectionString = configuration.GetConnectionString("RabbitMQ") ?? "amqp://guest:guest@rabbitmq:5672";
        var factory = new ConnectionFactory
        {
            Uri = new Uri(connectionString),
            AutomaticRecoveryEnabled = true,
            NetworkRecoveryInterval = TimeSpan.FromSeconds(10)
        };

        _connection = factory.CreateConnectionAsync().GetAwaiter().GetResult();
        _channel = _connection.CreateChannelAsync().GetAwaiter().GetResult();
        
        _channel.QueueDeclareAsync(
            queue: _queueName,
            durable: true,
            exclusive: false,
            autoDelete: false,
            arguments: null).GetAwaiter().GetResult();
    }

    public async Task PublishLogAsync(LogMessage logMessage)
    {
        try
        {
            var message = JsonSerializer.Serialize(logMessage);
            var body = Encoding.UTF8.GetBytes(message);

            await _channel.BasicPublishAsync(
                exchange: "",
                routingKey: _queueName,
                mandatory: true,
                basicProperties: new BasicProperties { Persistent = true },
                body: body);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to publish log to RabbitMQ");
        }
    }

    public void Dispose()
    {
        _channel?.CloseAsync().GetAwaiter().GetResult();
        _connection?.CloseAsync().GetAwaiter().GetResult();
    }
}