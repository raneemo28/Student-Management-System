namespace App.LoggingMicroservice.Models;

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