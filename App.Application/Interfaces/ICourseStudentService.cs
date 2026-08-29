using App.Application.DTOs;
using App.Application.Common;

namespace App.Application.Interfaces;

public interface ICourseStudentService
{
    Task<Result<IEnumerable<CourseStudentDto>>> GetAllAsync();
    Task<Result<CourseStudentDto>> GetByIdAsync(string studentId, string courseId);
    Task<Result<CourseStudentDto>> CreateAsync(CreateCourseStudentDto dto);
    Task<Result> DeleteAsync(string studentId, string courseId);
    Task<Result<CourseStudentDto>> UpdateMarksAsync(string studentId, string courseId, UpdateMarksDto dto);
    Task<Result<CourseStudentDto>> GetMarksAsync(string studentId, string courseId);
}
