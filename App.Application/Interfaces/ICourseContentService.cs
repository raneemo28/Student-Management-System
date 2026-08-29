using App.Application.DTOs;
using App.Application.Common;

namespace App.Application.Interfaces;

public interface ICourseContentService
{
    Task<Result<IEnumerable<CourseContentDto>>> GetByCourseAsync(string courseId);
    Task<Result<CourseContentDto>> GetByIdAsync(string id);
    Task<Result<CourseContentDto>> CreateAsync(string courseId, CreateCourseContentDto dto);
    Task<Result> DeleteAsync(string id);
}
