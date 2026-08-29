namespace App.Application.Common.Events;

public interface IDomainEvent
{
    Guid Id { get; }
    DateTime OccurredOn { get; }
    string EventType { get; }
    string Data { get; }
    bool IsProcessed { get; }
}
