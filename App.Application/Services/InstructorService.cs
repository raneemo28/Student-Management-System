using App.Application.DTOs;
using App.Application.Interfaces;
using App.Application.Common;
using App.Application.Repositories;
using App.domain.entity;

namespace App.Application.Services;

public class InstructorService : IInstructorService
{
    private readonly IUnitOfWork _unitOfWork;

    public InstructorService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<IEnumerable<InstructorDto>>> GetAllAsync()
    {
        try
        {
            var instructors = await _unitOfWork.Instructors.GetAllAsync();
            var instructorDtos = instructors.Select(i => new InstructorDto
            {
                Instructor_id = i.Instructor_id,
                FirstName = i.FirstName,
                LastName = i.LastName,
                User_id = i.User_id
            });

            return Result<IEnumerable<InstructorDto>>.Success(instructorDtos);
        }
        catch (Exception ex)
        {
            return Result<IEnumerable<InstructorDto>>.Failure(ex.Message);
        }
    }

    public async Task<Result<InstructorDto>> GetByIdAsync(string id)
    {
        try
        {
            var instructor = await _unitOfWork.Instructors.GetByIdAsync(id);
            if (instructor == null)
                return Result<InstructorDto>.Failure("Instructor not found");

            return Result<InstructorDto>.Success(new InstructorDto
            {
                Instructor_id = instructor.Instructor_id,
                FirstName = instructor.FirstName,
                LastName = instructor.LastName,
                User_id = instructor.User_id
            });
        }
        catch (Exception ex)
        {
            return Result<InstructorDto>.Failure(ex.Message);
        }
    }

    public async Task<Result<InstructorDto>> CreateAsync(CreateInstructorDto dto)
    {
        try
        {
            var instructor = new Instructor
            {
                FirstName = dto.FirstName,
                LastName = dto.LastName,
                Instructor_id = Guid.NewGuid().ToString()
            };

            await _unitOfWork.Instructors.AddAsync(instructor);
            await _unitOfWork.SaveChangesAsync();

            return Result<InstructorDto>.Success(new InstructorDto
            {
                Instructor_id = instructor.Instructor_id,
                FirstName = instructor.FirstName,
                LastName = instructor.LastName,
                User_id = instructor.User_id
            });
        }
        catch (Exception ex)
        {
            return Result<InstructorDto>.Failure(ex.Message);
        }
    }

    public async Task<Result<InstructorDto>> UpdateAsync(string id, UpdateInstructorDto dto)
    {
        try
        {
            var instructor = await _unitOfWork.Instructors.GetByIdAsync(id);
            if (instructor == null)
                return Result<InstructorDto>.Failure("Instructor not found");

            instructor.FirstName = dto.FirstName;
            instructor.LastName = dto.LastName;

            await _unitOfWork.Instructors.UpdateAsync(instructor);
            await _unitOfWork.SaveChangesAsync();

            return Result<InstructorDto>.Success(new InstructorDto
            {
                Instructor_id = instructor.Instructor_id,
                FirstName = instructor.FirstName,
                LastName = instructor.LastName,
                User_id = instructor.User_id
            });
        }
        catch (Exception ex)
        {
            return Result<InstructorDto>.Failure(ex.Message);
        }
    }

    public async Task<Result> DeleteAsync(string id)
    {
        try
        {
            var instructor = await _unitOfWork.Instructors.GetByIdAsync(id);
            if (instructor == null)
                return Result.Failure("Instructor not found");

            await _unitOfWork.Instructors.DeleteAsync(instructor);
            await _unitOfWork.SaveChangesAsync();
            return Result.Success();
        }
        catch (Exception ex)
        {
            return Result.Failure(ex.Message);
        }
    }
}
