using App.Application.DTOs;
using App.Application.Interfaces;
using App.Application.Common;
using App.Application.Repositories;
using App.domain.entity;
using Microsoft.AspNetCore.Identity;

namespace App.Application.Services;

public class EmployeeService : IEmployeeService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly UserManager<AppUser> _userManager;

    public EmployeeService(IUnitOfWork unitOfWork, UserManager<AppUser> userManager)
    {
        _unitOfWork = unitOfWork;
        _userManager = userManager;
    }

    public async Task<Result<IEnumerable<EmployeeDto>>> GetAllAsync()
    {
        try
        {
            var employees = await _unitOfWork.Employees.GetAllAsync();
            var employeeDtos = employees.Select(e => new EmployeeDto
            {
                Employee_id = e.Employee_id,
                FirstName = e.FirstName,
                LastName = e.LastName,
                User_id = e.User_id
            });

            return Result<IEnumerable<EmployeeDto>>.Success(employeeDtos);
        }
        catch (Exception ex)
        {
            return Result<IEnumerable<EmployeeDto>>.Failure(ex.Message);
        }
    }

    public async Task<Result<EmployeeDto>> GetByIdAsync(string id)
    {
        try
        {
            var employee = await _unitOfWork.Employees.GetByIdAsync(id);
            if (employee == null)
                return Result<EmployeeDto>.Failure("Employee not found");

            return Result<EmployeeDto>.Success(new EmployeeDto
            {
                Employee_id = employee.Employee_id,
                FirstName = employee.FirstName,
                LastName = employee.LastName,
                User_id = employee.User_id
            });
        }
        catch (Exception ex)
        {
            return Result<EmployeeDto>.Failure(ex.Message);
        }
    }

    public async Task<Result<EmployeeDto>> CreateAsync(CreateEmployeeDto dto)
    {
        try
        {
            var user = new AppUser
            {
                UserName = dto.Email,
                Email = dto.Email,
                CreatedAt = DateTime.UtcNow
            };

            var result = await _userManager.CreateAsync(user, dto.Password);
            if (!result.Succeeded)
            {
                var errors = string.Join(", ", result.Errors.Select(e => e.Description));
                return Result<EmployeeDto>.Failure(errors);
            }

            await _userManager.AddToRoleAsync(user, "Employee");

            var employee = new Employee
            {
                FirstName = dto.FirstName,
                LastName = dto.LastName,
                User_id = user.Id
            };

            await _unitOfWork.Employees.AddAsync(employee);
            await _unitOfWork.SaveChangesAsync();

            return Result<EmployeeDto>.Success(new EmployeeDto
            {
                Employee_id = employee.Employee_id,
                FirstName = employee.FirstName,
                LastName = employee.LastName,
                User_id = employee.User_id
            });
        }
        catch (Exception ex)
        {
            return Result<EmployeeDto>.Failure(ex.Message);
        }
    }

    public async Task<Result<EmployeeDto>> UpdateAsync(string id, UpdateEmployeeDto dto)
    {
        try
        {
            var employee = await _unitOfWork.Employees.GetByIdAsync(id);
            if (employee == null)
                return Result<EmployeeDto>.Failure("Employee not found");

            employee.FirstName = dto.FirstName;
            employee.LastName = dto.LastName;

            await _unitOfWork.Employees.UpdateAsync(employee);
            await _unitOfWork.SaveChangesAsync();

            return Result<EmployeeDto>.Success(new EmployeeDto
            {
                Employee_id = employee.Employee_id,
                FirstName = employee.FirstName,
                LastName = employee.LastName,
                User_id = employee.User_id
            });
        }
        catch (Exception ex)
        {
            return Result<EmployeeDto>.Failure(ex.Message);
        }
    }

    public async Task<Result> DeleteAsync(string id)
    {
        try
        {
            var employee = await _unitOfWork.Employees.GetByIdAsync(id);
            if (employee == null)
                return Result.Failure("Employee not found");

            await _unitOfWork.Employees.DeleteAsync(employee);
            await _unitOfWork.SaveChangesAsync();
            return Result.Success();
        }
        catch (Exception ex)
        {
            return Result.Failure(ex.Message);
        }
    }
}
