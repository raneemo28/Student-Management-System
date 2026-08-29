using App.Application.DTOs;
using App.Application.Interfaces;
using App.Application.Common;
using App.Application.Repositories;
using App.domain.entity;

namespace App.Application.Services;

public class AdvertisementService : IAdvertisementService
{
    private readonly IUnitOfWork _unitOfWork;

    public AdvertisementService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<IEnumerable<AdvertisementDto>>> GetAllAsync()
    {
        try
        {
            var advertisements = await _unitOfWork.Advertisements.GetAllAsync();
            var advertisementDtos = advertisements.Select(a => new AdvertisementDto
            {
                Advertisement_id = a.Advertisement_id,
                Title = a.Title,
                Description = a.Description,
                PublishedAt = a.PublishedAt,
                IsActive = a.IsActive
            });

            return Result<IEnumerable<AdvertisementDto>>.Success(advertisementDtos);
        }
        catch (Exception ex)
        {
            return Result<IEnumerable<AdvertisementDto>>.Failure(ex.Message);
        }
    }

    public async Task<Result<AdvertisementDto>> GetByIdAsync(string id)
    {
        try
        {
            var advertisement = await _unitOfWork.Advertisements.GetByIdAsync(id);
            if (advertisement == null)
                return Result<AdvertisementDto>.Failure("Advertisement not found");

            return Result<AdvertisementDto>.Success(new AdvertisementDto
            {
                Advertisement_id = advertisement.Advertisement_id,
                Title = advertisement.Title,
                Description = advertisement.Description,
                PublishedAt = advertisement.PublishedAt,
                IsActive = advertisement.IsActive
            });
        }
        catch (Exception ex)
        {
            return Result<AdvertisementDto>.Failure(ex.Message);
        }
    }

    public async Task<Result<AdvertisementDto>> CreateAsync(CreateAdvertisementDto dto)
    {
        try
        {
            var advertisement = new Advertisement
            {
                Title = dto.Title,
                Description = dto.Description,
                PublishedAt = DateTime.UtcNow,
                IsActive = true,
                Advertisement_id = Guid.NewGuid().ToString()
            };

            await _unitOfWork.Advertisements.AddAsync(advertisement);
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
        catch (Exception ex)
        {
            return Result<AdvertisementDto>.Failure(ex.Message);
        }
    }

    public async Task<Result<AdvertisementDto>> UpdateAsync(string id, UpdateAdvertisementDto dto)
    {
        try
        {
            var advertisement = await _unitOfWork.Advertisements.GetByIdAsync(id);
            if (advertisement == null)
                return Result<AdvertisementDto>.Failure("Advertisement not found");

            advertisement.Title = dto.Title;
            advertisement.Description = dto.Description;
            advertisement.IsActive = dto.IsActive;

            await _unitOfWork.Advertisements.UpdateAsync(advertisement);
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
        catch (Exception ex)
        {
            return Result<AdvertisementDto>.Failure(ex.Message);
        }
    }

    public async Task<Result> DeleteAsync(string id)
    {
        try
        {
            var advertisement = await _unitOfWork.Advertisements.GetByIdAsync(id);
            if (advertisement == null)
                return Result.Failure("Advertisement not found");

            await _unitOfWork.Advertisements.DeleteAsync(advertisement);
            await _unitOfWork.SaveChangesAsync();
            return Result.Success();
        }
        catch (Exception ex)
        {
            return Result.Failure(ex.Message);
        }
    }
}
