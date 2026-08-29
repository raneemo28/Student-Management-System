using App.Application.DTOs;
using App.Application.Interfaces;
using App.Application.Common;
using App.Application.Repositories;
using App.domain.entity;
using Microsoft.EntityFrameworkCore;

namespace App.Application.Services;

public class StudentService : IStudentService
{
    private readonly IUnitOfWork _unitOfWork;

    public StudentService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<IEnumerable<StudentDto>>> GetAllAsync()
    {
        try
        {
            var students = await _unitOfWork.Students.GetAllAsync();
            var studentDtos = students.Select(s => new StudentDto
            {
                Student_id = s.Student_id,
                FirstName = s.FirstName,
                LastName = s.LastName,
                User_id = s.User_id
            });

            return Result<IEnumerable<StudentDto>>.Success(studentDtos);
        }
        catch (Exception ex)
        {
            return Result<IEnumerable<StudentDto>>.Failure(ex.Message);
        }
    }

    public async Task<Result<StudentDto>> GetByIdAsync(string id)
    {
        try
        {
            var student = await _unitOfWork.Students.GetByIdAsync(id);
            if (student == null)
                return Result<StudentDto>.Failure("Student not found");

            return Result<StudentDto>.Success(new StudentDto
            {
                Student_id = student.Student_id,
                FirstName = student.FirstName,
                LastName = student.LastName,
                User_id = student.User_id
            });
        }
        catch (Exception ex)
        {
            return Result<StudentDto>.Failure(ex.Message);
        }
    }

    public async Task<Result<StudentDto>> CreateAsync(CreateStudentDto dto)
    {
        try
        {
            var student = new Student
            {
                FirstName = dto.FirstName,
                LastName = dto.LastName,
                Student_id = Guid.NewGuid().ToString()
            };

            await _unitOfWork.Students.AddAsync(student);
            await _unitOfWork.SaveChangesAsync();

            return Result<StudentDto>.Success(new StudentDto
            {
                Student_id = student.Student_id,
                FirstName = student.FirstName,
                LastName = student.LastName,
                User_id = student.User_id
            });
        }
        catch (Exception ex)
        {
            return Result<StudentDto>.Failure(ex.Message);
        }
    }

    public async Task<Result<StudentDto>> UpdateAsync(string id, UpdateStudentDto dto)
    {
        try
        {
            var student = await _unitOfWork.Students.GetByIdAsync(id);
            if (student == null)
                return Result<StudentDto>.Failure("Student not found");

            student.FirstName = dto.FirstName;
            student.LastName = dto.LastName;

            await _unitOfWork.Students.UpdateAsync(student);
            await _unitOfWork.SaveChangesAsync();

            return Result<StudentDto>.Success(new StudentDto
            {
                Student_id = student.Student_id,
                FirstName = student.FirstName,
                LastName = student.LastName,
                User_id = student.User_id
            });
        }
        catch (Exception ex)
        {
            return Result<StudentDto>.Failure(ex.Message);
        }
    }

    public async Task<Result> DeleteAsync(string id)
    {
        try
        {
            var student = await _unitOfWork.Students.GetByIdAsync(id);
            if (student == null)
                return Result.Failure("Student not found");

            await _unitOfWork.Students.DeleteAsync(student);
            await _unitOfWork.SaveChangesAsync();
            return Result.Success();
        }
        catch (Exception ex)
        {
            return Result.Failure(ex.Message);
        }
    }
}
