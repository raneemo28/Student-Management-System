namespace App.domain.entity;

public class DomainEvent
{
    public Guid Id { get; set; }
    public string EventType { get; set; } = string.Empty;
    public string Data { get; set; } = string.Empty;
    public bool IsProcessed { get; set; }
    public DateTime OccurredOn { get; set; }
    public DateTime? ProcessedOn { get; set; }
}
