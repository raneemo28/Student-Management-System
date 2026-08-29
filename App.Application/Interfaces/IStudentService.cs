using App.Application.DTOs;
using App.Application.Common;

namespace App.Application.Interfaces;

public interface IStudentService
{
    Task<Result<IEnumerable<StudentDto>>> GetAllAsync();
    Task<Result<StudentDto>> GetByIdAsync(string id);
    Task<Result<StudentDto>> CreateAsync(CreateStudentDto dto);
    Task<Result<StudentDto>> UpdateAsync(string id, UpdateStudentDto dto);
    Task<Result> DeleteAsync(string id);
}
