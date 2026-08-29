using App.Application.DTOs;
using App.Application.Common;

namespace App.Application.Interfaces;

public interface IHomeworkSolutionService
{
    Task<Result<IEnumerable<HomeworkSolutionDto>>> GetByHomeworkAsync(string homeworkId);
    Task<Result<HomeworkSolutionDto>> GetByIdAsync(string id);
    Task<Result<HomeworkSolutionDto>> CreateAsync(CreateHomeworkSolutionDto dto);
    Task<Result> DeleteAsync(string id);
}
