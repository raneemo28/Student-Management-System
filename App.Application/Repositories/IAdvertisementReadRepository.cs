using App.domain.ReadModels;

namespace App.Application.Repositories;

public interface IAdvertisementReadRepository
{
    Task<IEnumerable<AdvertisementRead>> GetAllAsync();
    Task<AdvertisementRead?> GetByIdAsync(string id);
}
