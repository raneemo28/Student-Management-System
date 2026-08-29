using App.Application.DTOs;
using App.Application.Repositories;
using MediatR;

namespace App.Application.CQRS.Advertisements.Queries;

public record GetAllAdvertisementsQuery() : IRequest<IEnumerable<AdvertisementDto>>;

public class GetAllAdvertisementsQueryHandler : IRequestHandler<GetAllAdvertisementsQuery, IEnumerable<AdvertisementDto>>
{
    private readonly IAdvertisementReadRepository _readRepository;

    public GetAllAdvertisementsQueryHandler(IAdvertisementReadRepository readRepository)
    {
        _readRepository = readRepository;
    }

    public async Task<IEnumerable<AdvertisementDto>> Handle(GetAllAdvertisementsQuery request, CancellationToken cancellationToken)
    {
        var advertisements = await _readRepository.GetAllAsync();
        return advertisements.Select(a => new AdvertisementDto
        {
            Advertisement_id = a.Advertisement_id,
            Title = a.Title,
            Description = a.Description,
            PublishedAt = a.PublishedAt,
            IsActive = a.IsActive
        });
    }
}
