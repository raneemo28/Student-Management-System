using App.Application.DTOs;
using App.Application.Common;

namespace App.Application.Interfaces;

public interface IInstructorService
{
    Task<Result<IEnumerable<InstructorDto>>> GetAllAsync();
    Task<Result<InstructorDto>> GetByIdAsync(string id);
    Task<Result<InstructorDto>> CreateAsync(CreateInstructorDto dto);
    Task<Result<InstructorDto>> UpdateAsync(string id, UpdateInstructorDto dto);
    Task<Result> DeleteAsync(string id);
}
