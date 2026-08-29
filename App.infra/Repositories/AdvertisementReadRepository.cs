using App.Application.Repositories;
using App.domain.ReadModels;
using App.infra.Persistence;
using Microsoft.EntityFrameworkCore;

namespace App.infra.Repositories;

public class AdvertisementReadRepository : IAdvertisementReadRepository
{
    private readonly AppReadDbContext _context;

    public AdvertisementReadRepository(AppReadDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<AdvertisementRead>> GetAllAsync()
    {
        return await _context.AdvertisementsRead.ToListAsync();
    }

    public async Task<AdvertisementRead?> GetByIdAsync(string id)
    {
        return await _context.AdvertisementsRead.FindAsync(id);
    }
}
