using App.Application.Common;
using App.Application.Common.Events;
using App.Application.Repositories;
using MediatR;

namespace App.Application.CQRS.Advertisements.Commands;

public record DeleteAdvertisementCommand(string Id) : IRequest<Result>;

public class DeleteAdvertisementCommandHandler : IRequestHandler<DeleteAdvertisementCommand, Result>
{
    private readonly IUnitOfWork _unitOfWork;

    public DeleteAdvertisementCommandHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<Result> Handle(DeleteAdvertisementCommand request, CancellationToken cancellationToken)
    {
        var advertisement = await _unitOfWork.Advertisements.GetByIdAsync(request.Id);
        if (advertisement == null)
            return Result.Failure("Advertisement not found");

        await _unitOfWork.Advertisements.DeleteAsync(advertisement);

        var domainEvent = new App.domain.entity.DomainEvent
        {
            Id = Guid.NewGuid(),
            EventType = "AdvertisementDeleted",
            Data = DomainEventSerializer.Serialize(new { advertisement.Advertisement_id }),
            OccurredOn = DateTime.UtcNow,
            IsProcessed = false
        };

        await _unitOfWork.DomainEvents.AddAsync(domainEvent);
        await _unitOfWork.SaveChangesAsync();

        return Result.Success();
    }
}
