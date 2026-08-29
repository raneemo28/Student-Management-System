using App.Application.DTOs;
using App.Application.Common;

namespace App.Application.Interfaces;

public interface IAdvertisementService
{
    Task<Result<IEnumerable<AdvertisementDto>>> GetAllAsync();
    Task<Result<AdvertisementDto>> GetByIdAsync(string id);
    Task<Result<AdvertisementDto>> CreateAsync(CreateAdvertisementDto dto);
    Task<Result<AdvertisementDto>> UpdateAsync(string id, UpdateAdvertisementDto dto);
    Task<Result> DeleteAsync(string id);
}
