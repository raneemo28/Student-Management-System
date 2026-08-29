using App.Application.DTOs;
using App.Application.Common;

namespace App.Application.Interfaces;

public interface IEmployeeService
{
    Task<Result<IEnumerable<EmployeeDto>>> GetAllAsync();
    Task<Result<EmployeeDto>> GetByIdAsync(string id);
    Task<Result<EmployeeDto>> CreateAsync(CreateEmployeeDto dto);
    Task<Result<EmployeeDto>> UpdateAsync(string id, UpdateEmployeeDto dto);
    Task<Result> DeleteAsync(string id);
}
