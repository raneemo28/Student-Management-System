using App.Application.DTOs;
using App.Application.Repositories;
using MediatR;

namespace App.Application.CQRS.Advertisements.Queries;

public record GetAdvertisementByIdQuery(string Id) : IRequest<AdvertisementDto?>;

public class GetAdvertisementByIdQueryHandler : IRequestHandler<GetAdvertisementByIdQuery, AdvertisementDto?>
{
    private readonly IAdvertisementReadRepository _readRepository;

    public GetAdvertisementByIdQueryHandler(IAdvertisementReadRepository readRepository)
    {
        _readRepository = readRepository;
    }

    public async Task<AdvertisementDto?> Handle(GetAdvertisementByIdQuery request, CancellationToken cancellationToken)
    {
        var advertisement = await _readRepository.GetByIdAsync(request.Id);
        if (advertisement == null) return null;

        return new AdvertisementDto
        {
            Advertisement_id = advertisement.Advertisement_id,
            Title = advertisement.Title,
            Description = advertisement.Description,
            PublishedAt = advertisement.PublishedAt,
            IsActive = advertisement.IsActive
        };
    }
}
