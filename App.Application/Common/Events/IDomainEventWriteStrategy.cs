using App.domain.entity;

namespace App.Application.Common.Events;

public interface IDomainEventWriteStrategy
{
    Task ProcessAsync(DomainEvent domainEvent);
}
