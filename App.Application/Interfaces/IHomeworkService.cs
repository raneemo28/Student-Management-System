using App.Application.DTOs;
using App.Application.Common;

namespace App.Application.Interfaces;

public interface IHomeworkService
{
    Task<Result<IEnumerable<HomeworkDto>>> GetByCourseAsync(string courseId);
    Task<Result<HomeworkDto>> GetByIdAsync(string id);
    Task<Result<HomeworkDto>> CreateAsync(string courseId, CreateHomeworkDto dto);
    Task<Result<HomeworkDto>> UpdateAsync(string id, UpdateHomeworkDto dto);
    Task<Result> DeleteAsync(string id);
}
