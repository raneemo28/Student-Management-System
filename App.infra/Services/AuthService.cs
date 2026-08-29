using App.Application.DTOs;
using App.Application.Interfaces;
using App.Application.Common;
using App.Application.Repositories;
using App.domain.entity;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace App.infra.Services;

public class AuthService : IAuthService
{
    private readonly UserManager<AppUser> _userManager;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IConfiguration _configuration;

    public AuthService(UserManager<AppUser> userManager, IUnitOfWork unitOfWork, IConfiguration configuration)
    {
        _userManager = userManager;
        _unitOfWork = unitOfWork;
        _configuration = configuration;
    }

    public async Task<Result<AuthResponseDto>> RegisterAsync(RegisterUserDto dto)
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
                return Result<AuthResponseDto>.Failure(errors);
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

            var token = GenerateJwtToken(user, employeeId: employee.Employee_id);
            return Result<AuthResponseDto>.Success(new AuthResponseDto
            {
                Token = token,
                Email = user.Email!,
                UserId = user.Id,
                EmployeeId = employee.Employee_id
            });
        }
        catch (Exception ex)
        {
            return Result<AuthResponseDto>.Failure(ex.Message);
        }
    }

    public async Task<Result<AuthResponseDto>> LoginAsync(LoginUserDto dto)
    {
        try
        {
            var user = await _userManager.FindByEmailAsync(dto.Email);
            if (user == null)
                return Result<AuthResponseDto>.Failure("Invalid email or password");

            var isValid = await _userManager.CheckPasswordAsync(user, dto.Password);
            if (!isValid)
                return Result<AuthResponseDto>.Failure("Invalid email or password");

            string? studentId = null;
            string? instructorId = null;
            string? employeeId = null;

            var students = await _unitOfWork.Students.GetAllAsync();
            var student = students.FirstOrDefault(s => s.User_id == user.Id);
            if (student != null) studentId = student.Student_id;

            var instructors = await _unitOfWork.Instructors.GetAllAsync();
            var instructor = instructors.FirstOrDefault(i => i.User_id == user.Id);
            if (instructor != null) instructorId = instructor.Instructor_id;

            var employees = await _unitOfWork.Employees.GetAllAsync();
            var employee = employees.FirstOrDefault(e => e.User_id == user.Id);
            if (employee != null) employeeId = employee.Employee_id;

            var token = GenerateJwtToken(user, studentId, instructorId, employeeId);
            return Result<AuthResponseDto>.Success(new AuthResponseDto
            {
                Token = token,
                Email = user.Email!,
                UserId = user.Id,
                StudentId = studentId ?? string.Empty,
                InstructorId = instructorId ?? string.Empty,
                EmployeeId = employeeId ?? string.Empty
            });
        }
        catch (Exception ex)
        {
            return Result<AuthResponseDto>.Failure(ex.Message);
        }
    }

    private string GenerateJwtToken(AppUser user, string? studentId = null, string? instructorId = null, string? employeeId = null)
    {
        var jwtSettings = _configuration.GetSection("JwtSettings");
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings["Secret"]!));
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, user.Id),
            new Claim(ClaimTypes.Email, user.Email!)
        };

        if (!string.IsNullOrEmpty(studentId))
        {
            claims.Add(new Claim("StudentId", studentId));
        }

        if (!string.IsNullOrEmpty(instructorId))
        {
            claims.Add(new Claim("InstructorId", instructorId));
        }

        if (!string.IsNullOrEmpty(employeeId))
        {
            claims.Add(new Claim("EmployeeId", employeeId));
        }

        var token = new JwtSecurityToken(
            issuer: jwtSettings["Issuer"],
            audience: jwtSettings["Audience"],
            claims: claims,
            expires: DateTime.UtcNow.AddHours(Convert.ToDouble(jwtSettings["ExpiryInHours"])),
            signingCredentials: credentials
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}
