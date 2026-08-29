using App.Application.DTOs;
using App.Application.Common;
using App.Application.Common.Events;
using App.Application.Repositories;
using MediatR;

namespace App.Application.CQRS.Advertisements.Commands;

public record CreateAdvertisementCommand(CreateAdvertisementDto Dto) : IRequest<Result<AdvertisementDto>>;

public class CreateAdvertisementCommandHandler : IRequestHandler<CreateAdvertisementCommand, Result<AdvertisementDto>>
{
    private readonly IUnitOfWork _unitOfWork;

    public CreateAdvertisementCommandHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<AdvertisementDto>> Handle(CreateAdvertisementCommand request, CancellationToken cancellationToken)
    {
        var advertisement = new App.domain.entity.Advertisement
        {
            Title = request.Dto.Title,
            Description = request.Dto.Description,
            PublishedAt = DateTime.UtcNow,
            IsActive = true,
            Advertisement_id = Guid.NewGuid().ToString()
        };

        await _unitOfWork.Advertisements.AddAsync(advertisement);

        var domainEvent = new App.domain.entity.DomainEvent
        {
            Id = Guid.NewGuid(),
            EventType = "AdvertisementCreated",
            Data = DomainEventSerializer.Serialize(new { advertisement.Advertisement_id, advertisement.Title, advertisement.Description, advertisement.PublishedAt, advertisement.IsActive }),
            OccurredOn = DateTime.UtcNow,
            IsProcessed = false
        };

        await _unitOfWork.DomainEvents.AddAsync(domainEvent);
        await _unitOfWork.SaveChangesAsync();

        return Result<AdvertisementDto>.Success(new AdvertisementDto
        {
            Advertisement_id = advertisement.Advertisement_id,
            Title = advertisement.Title,
            Description = advertisement.Description,
            PublishedAt = advertisement.PublishedAt,
            IsActive = advertisement.IsActive
        });
    }
}
