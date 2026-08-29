using App.Application.DTOs;
using App.Application.Interfaces;
using App.Application.Common;
using App.Application.Repositories;

namespace App.Application.Services;

public class CourseContentService : ICourseContentService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IFileStorageService _fileStorageService;

    public CourseContentService(IUnitOfWork unitOfWork, IFileStorageService fileStorageService)
    {
        _unitOfWork = unitOfWork;
        _fileStorageService = fileStorageService;
    }

    public async Task<Result<IEnumerable<CourseContentDto>>> GetByCourseAsync(string courseId)
    {
        try
        {
            var contents = await _unitOfWork.CourseContents.GetByCourseIdAsync(courseId);
            var contentDtos = contents.Select(c => new CourseContentDto
            {
                Content_id = c.Content_id,
                Course_id = c.Course_id,
                Title = c.Title,
                Description = c.Description,
                ContentType = c.ContentType,
                FileName = c.FileName,
                FileUrl = c.FileUrl,
                FileSize = c.FileSize,
                CreatedAt = c.CreatedAt
            });

            return Result<IEnumerable<CourseContentDto>>.Success(contentDtos);
        }
        catch (Exception ex)
        {
            return Result<IEnumerable<CourseContentDto>>.Failure(ex.Message);
        }
    }

    public async Task<Result<CourseContentDto>> GetByIdAsync(string id)
    {
        try
        {
            var content = await _unitOfWork.CourseContents.GetByIdAsync(id);
            if (content == null)
                return Result<CourseContentDto>.Failure("Course content not found");

            return Result<CourseContentDto>.Success(new CourseContentDto
            {
                Content_id = content.Content_id,
                Course_id = content.Course_id,
                Title = content.Title,
                Description = content.Description,
                ContentType = content.ContentType,
                FileName = content.FileName,
                FileUrl = content.FileUrl,
                FileSize = content.FileSize,
                CreatedAt = content.CreatedAt
            });
        }
        catch (Exception ex)
        {
            return Result<CourseContentDto>.Failure(ex.Message);
        }
    }

    public async Task<Result<CourseContentDto>> CreateAsync(string courseId, CreateCourseContentDto dto)
    {
        try
        {
            var fileUrl = await _fileStorageService.SaveFileAsync(new MemoryStream(dto.FileBytes), dto.FileName, dto.ContentType);

            var courseContent = new App.domain.entity.CourseContent
            {
                Course_id = courseId,
                Title = dto.Title,
                Description = dto.Description,
                ContentType = dto.ContentType,
                FileName = dto.FileName,
                FileUrl = fileUrl,
                FileSize = dto.FileSize,
                Content_id = Guid.NewGuid().ToString(),
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            await _unitOfWork.CourseContents.AddAsync(courseContent);
            await _unitOfWork.SaveChangesAsync();

            return Result<CourseContentDto>.Success(new CourseContentDto
            {
                Content_id = courseContent.Content_id,
                Course_id = courseContent.Course_id,
                Title = courseContent.Title,
                Description = courseContent.Description,
                ContentType = courseContent.ContentType,
                FileName = courseContent.FileName,
                FileUrl = courseContent.FileUrl,
                FileSize = courseContent.FileSize,
                CreatedAt = courseContent.CreatedAt
            });
        }
        catch (Exception ex)
        {
            return Result<CourseContentDto>.Failure(ex.Message);
        }
    }

    public async Task<Result> DeleteAsync(string id)
    {
        try
        {
            var courseContent = await _unitOfWork.CourseContents.GetByIdAsync(id);
            if (courseContent == null)
                return Result.Failure("Course content not found");

            await _fileStorageService.DeleteFileAsync(courseContent.FileUrl);
            await _unitOfWork.CourseContents.DeleteAsync(courseContent);
            await _unitOfWork.SaveChangesAsync();

            return Result.Success();
        }
        catch (Exception ex)
        {
            return Result.Failure(ex.Message);
        }
    }
}
