using App.Application.DTOs;
using App.Application.Common;

namespace App.Application.Interfaces;

public interface ICourseService
{
    Task<Result<IEnumerable<CourseDto>>> GetAllAsync();
    Task<Result<CourseDto>> GetByIdAsync(string id);
    Task<Result<CourseDto>> CreateAsync(CreateCourseDto dto);
    Task<Result<CourseDto>> UpdateAsync(string id, UpdateCourseDto dto);
    Task<Result> DeleteAsync(string id);
    Task<Result> UpdateWeightsAsync(string id, CourseWeightsDto dto);
}
