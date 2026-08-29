using App.Application.DTOs;
using App.Application.Interfaces;
using App.Application.Common;
using App.Application.Repositories;
using App.domain.entity;

namespace App.Application.Services;

public class HomeworkService : IHomeworkService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IFileStorageService _fileStorageService;

    public HomeworkService(IUnitOfWork unitOfWork, IFileStorageService fileStorageService)
    {
        _unitOfWork = unitOfWork;
        _fileStorageService = fileStorageService;
    }

    public async Task<Result<IEnumerable<HomeworkDto>>> GetByCourseAsync(string courseId)
    {
        try
        {
            var homeworks = await _unitOfWork.Homeworks.GetByCourseIdAsync(courseId);
            var homeworkDtos = homeworks.Select(h => new HomeworkDto
            {
                Homework_id = h.Homework_id,
                Course_id = h.Course_id,
                Title = h.Title,
                Description = h.Description,
                QuestionFileName = h.QuestionFileName,
                QuestionFileUrl = h.QuestionFileUrl,
                DueDate = h.DueDate,
                TotalMarks = h.TotalMarks,
                CreatedAt = h.CreatedAt
            });

            return Result<IEnumerable<HomeworkDto>>.Success(homeworkDtos);
        }
        catch (Exception ex)
        {
            return Result<IEnumerable<HomeworkDto>>.Failure(ex.Message);
        }
    }

    public async Task<Result<HomeworkDto>> GetByIdAsync(string id)
    {
        try
        {
            var homework = await _unitOfWork.Homeworks.GetByIdAsync(id);
            if (homework == null)
                return Result<HomeworkDto>.Failure("Homework not found");

            return Result<HomeworkDto>.Success(new HomeworkDto
            {
                Homework_id = homework.Homework_id,
                Course_id = homework.Course_id,
                Title = homework.Title,
                Description = homework.Description,
                QuestionFileName = homework.QuestionFileName,
                QuestionFileUrl = homework.QuestionFileUrl,
                DueDate = homework.DueDate,
                TotalMarks = homework.TotalMarks,
                CreatedAt = homework.CreatedAt
            });
        }
        catch (Exception ex)
        {
            return Result<HomeworkDto>.Failure(ex.Message);
        }
    }

    public async Task<Result<HomeworkDto>> CreateAsync(string courseId, CreateHomeworkDto dto)
    {
        try
        {
            var fileUrl = await _fileStorageService.SaveFileAsync(
                new MemoryStream(dto.QuestionFileBytes),
                dto.QuestionFileName,
                "Pdf");

            var homework = new Homework
            {
                Course_id = courseId,
                Title = dto.Title,
                Description = dto.Description,
                DueDate = dto.DueDate,
                TotalMarks = dto.TotalMarks,
                QuestionFileName = dto.QuestionFileName,
                QuestionFileUrl = fileUrl,
                Homework_id = Guid.NewGuid().ToString(),
                CreatedAt = DateTime.UtcNow
            };

            await _unitOfWork.Homeworks.AddAsync(homework);
            await _unitOfWork.SaveChangesAsync();

            return Result<HomeworkDto>.Success(new HomeworkDto
            {
                Homework_id = homework.Homework_id,
                Course_id = homework.Course_id,
                Title = homework.Title,
                Description = homework.Description,
                QuestionFileName = homework.QuestionFileName,
                QuestionFileUrl = homework.QuestionFileUrl,
                DueDate = homework.DueDate,
                TotalMarks = homework.TotalMarks,
                CreatedAt = homework.CreatedAt
            });
        }
        catch (Exception ex)
        {
            return Result<HomeworkDto>.Failure(ex.Message);
        }
    }

    public async Task<Result<HomeworkDto>> UpdateAsync(string id, UpdateHomeworkDto dto)
    {
        try
        {
            var homework = await _unitOfWork.Homeworks.GetByIdAsync(id);
            if (homework == null)
                return Result<HomeworkDto>.Failure("Homework not found");

            homework.Title = dto.Title;
            homework.Description = dto.Description;
            homework.DueDate = dto.DueDate;
            homework.TotalMarks = dto.TotalMarks;

            if (dto.QuestionFileBytes != null && dto.QuestionFileBytes.Length > 0)
            {
                await _fileStorageService.DeleteFileAsync(homework.QuestionFileUrl);
                homework.QuestionFileUrl = await _fileStorageService.SaveFileAsync(
                    new MemoryStream(dto.QuestionFileBytes),
                    dto.QuestionFileName!,
                    "Pdf");
                homework.QuestionFileName = dto.QuestionFileName!;
            }

            await _unitOfWork.Homeworks.UpdateAsync(homework);
            await _unitOfWork.SaveChangesAsync();

            return Result<HomeworkDto>.Success(new HomeworkDto
            {
                Homework_id = homework.Homework_id,
                Course_id = homework.Course_id,
                Title = homework.Title,
                Description = homework.Description,
                QuestionFileName = homework.QuestionFileName,
                QuestionFileUrl = homework.QuestionFileUrl,
                DueDate = homework.DueDate,
                TotalMarks = homework.TotalMarks,
                CreatedAt = homework.CreatedAt
            });
        }
        catch (Exception ex)
        {
            return Result<HomeworkDto>.Failure(ex.Message);
        }
    }

    public async Task<Result> DeleteAsync(string id)
    {
        try
        {
            var homework = await _unitOfWork.Homeworks.GetByIdAsync(id);
            if (homework == null)
                return Result.Failure("Homework not found");

            await _fileStorageService.DeleteFileAsync(homework.QuestionFileUrl);
            await _unitOfWork.Homeworks.DeleteAsync(homework);
            await _unitOfWork.SaveChangesAsync();

            return Result.Success();
        }
        catch (Exception ex)
        {
            return Result.Failure(ex.Message);
        }
    }
}
