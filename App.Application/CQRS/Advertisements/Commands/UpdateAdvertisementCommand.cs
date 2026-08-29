using App.Application.DTOs;
using App.Application.Common;
using App.Application.Common.Events;
using App.Application.Repositories;
using MediatR;

namespace App.Application.CQRS.Advertisements.Commands;

public record UpdateAdvertisementCommand(string Id, UpdateAdvertisementDto Dto) : IRequest<Result<AdvertisementDto>>;

public class UpdateAdvertisementCommandHandler : IRequestHandler<UpdateAdvertisementCommand, Result<AdvertisementDto>>
{
    private readonly IUnitOfWork _unitOfWork;

    public UpdateAdvertisementCommandHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<AdvertisementDto>> Handle(UpdateAdvertisementCommand request, CancellationToken cancellationToken)
    {
        var advertisement = await _unitOfWork.Advertisements.GetByIdAsync(request.Id);
        if (advertisement == null)
            return Result<AdvertisementDto>.Failure("Advertisement not found");

        advertisement.Title = request.Dto.Title;
        advertisement.Description = request.Dto.Description;
        advertisement.IsActive = request.Dto.IsActive;

        await _unitOfWork.Advertisements.UpdateAsync(advertisement);

        var domainEvent = new App.domain.entity.DomainEvent
        {
            Id = Guid.NewGuid(),
            EventType = "AdvertisementUpdated",
            Data = DomainEventSerializer.Serialize(new { advertisement.Advertisement_id, advertisement.Title, advertisement.Description, advertisement.IsActive }),
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
