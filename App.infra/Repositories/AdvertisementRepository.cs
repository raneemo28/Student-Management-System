using App.Application.Repositories;
using App.domain.entity;
using App.infra.Persistence;
using Microsoft.EntityFrameworkCore;

namespace App.infra.Repositories;

public class AdvertisementRepository : Repository<Advertisement>, IAdvertisementRepository
{
    public AdvertisementRepository(AppDbContext context) : base(context) { }
}
