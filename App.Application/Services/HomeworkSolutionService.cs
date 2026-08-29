using App.Application.DTOs;
using App.Application.Interfaces;
using App.Application.Common;
using App.Application.Repositories;
using App.domain.entity;

namespace App.Application.Services;

public class HomeworkSolutionService : IHomeworkSolutionService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IFileStorageService _fileStorageService;

    public HomeworkSolutionService(IUnitOfWork unitOfWork, IFileStorageService fileStorageService)
    {
        _unitOfWork = unitOfWork;
        _fileStorageService = fileStorageService;
    }

    public async Task<Result<IEnumerable<HomeworkSolutionDto>>> GetByHomeworkAsync(string homeworkId)
    {
        try
        {
            var solutions = await _unitOfWork.HomeworkSolutions.GetByHomeworkIdAsync(homeworkId);
            var solutionDtos = solutions.Select(s => new HomeworkSolutionDto
            {
                Solution_id = s.Solution_id,
                Homework_id = s.Homework_id,
                SolutionFileName = s.SolutionFileName,
                SolutionFileUrl = s.SolutionFileUrl,
                UploadedAt = s.UploadedAt,
                Notes = s.Notes
            });

            return Result<IEnumerable<HomeworkSolutionDto>>.Success(solutionDtos);
        }
        catch (Exception ex)
        {
            return Result<IEnumerable<HomeworkSolutionDto>>.Failure(ex.Message);
        }
    }

    public async Task<Result<HomeworkSolutionDto>> GetByIdAsync(string id)
    {
        try
        {
            var solution = await _unitOfWork.HomeworkSolutions.GetByIdAsync(id);
            if (solution == null)
                return Result<HomeworkSolutionDto>.Failure("Solution not found");

            return Result<HomeworkSolutionDto>.Success(new HomeworkSolutionDto
            {
                Solution_id = solution.Solution_id,
                Homework_id = solution.Homework_id,
                SolutionFileName = solution.SolutionFileName,
                SolutionFileUrl = solution.SolutionFileUrl,
                UploadedAt = solution.UploadedAt,
                Notes = solution.Notes
            });
        }
        catch (Exception ex)
        {
            return Result<HomeworkSolutionDto>.Failure(ex.Message);
        }
    }

    public async Task<Result<HomeworkSolutionDto>> CreateAsync(CreateHomeworkSolutionDto dto)
    {
        try
        {
            var fileUrl = await _fileStorageService.SaveFileAsync(
                new MemoryStream(dto.SolutionFileBytes),
                dto.SolutionFileName,
                "Pdf");

            var solution = new HomeworkSolution
            {
                Homework_id = dto.Homework_id,
                SolutionFileName = dto.SolutionFileName,
                SolutionFileUrl = fileUrl,
                Notes = dto.Notes,
                UploadedAt = DateTime.UtcNow,
                Solution_id = Guid.NewGuid().ToString()
            };

            await _unitOfWork.HomeworkSolutions.AddAsync(solution);
            await _unitOfWork.SaveChangesAsync();

            return Result<HomeworkSolutionDto>.Success(new HomeworkSolutionDto
            {
                Solution_id = solution.Solution_id,
                Homework_id = solution.Homework_id,
                SolutionFileName = solution.SolutionFileName,
                SolutionFileUrl = solution.SolutionFileUrl,
                UploadedAt = solution.UploadedAt,
                Notes = solution.Notes
            });
        }
        catch (Exception ex)
        {
            return Result<HomeworkSolutionDto>.Failure(ex.Message);
        }
    }

    public async Task<Result> DeleteAsync(string id)
    {
        try
        {
            var solution = await _unitOfWork.HomeworkSolutions.GetByIdAsync(id);
            if (solution == null)
                return Result.Failure("Solution not found");

            await _fileStorageService.DeleteFileAsync(solution.SolutionFileUrl);
            await _unitOfWork.HomeworkSolutions.DeleteAsync(solution);
            await _unitOfWork.SaveChangesAsync();

            return Result.Success();
        }
        catch (Exception ex)
        {
            return Result.Failure(ex.Message);
        }
    }
}
