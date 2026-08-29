using App.Application.DTOs;
using App.Application.Interfaces;
using App.Application.Common;
using App.Application.Repositories;
using App.domain.entity;

namespace App.Application.Services;

public class CourseService : ICourseService
{
    private readonly IUnitOfWork _unitOfWork;

    public CourseService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<IEnumerable<CourseDto>>> GetAllAsync()
    {
        try
        {
            var courses = await _unitOfWork.Courses.GetAllAsync();
            var courseDtos = courses.Select(c => new CourseDto
            {
                Course_id = c.Course_id,
                Course_name = c.Course_name,
                Instructor_id = c.Instructor_id,
                WorkWeight = c.WorkWeight,
                FirstExamWeight = c.FirstExamWeight,
                SecondExamWeight = c.SecondExamWeight,
                FinalExamWeight = c.FinalExamWeight
            });

            return Result<IEnumerable<CourseDto>>.Success(courseDtos);
        }
        catch (Exception ex)
        {
            return Result<IEnumerable<CourseDto>>.Failure(ex.Message);
        }
    }

    public async Task<Result<CourseDto>> GetByIdAsync(string id)
    {
        try
        {
            var course = await _unitOfWork.Courses.GetByIdAsync(id);
            if (course == null)
                return Result<CourseDto>.Failure("Course not found");

            return Result<CourseDto>.Success(new CourseDto
            {
                Course_id = course.Course_id,
                Course_name = course.Course_name,
                Instructor_id = course.Instructor_id,
                WorkWeight = course.WorkWeight,
                FirstExamWeight = course.FirstExamWeight,
                SecondExamWeight = course.SecondExamWeight,
                FinalExamWeight = course.FinalExamWeight
            });
        }
        catch (Exception ex)
        {
            return Result<CourseDto>.Failure(ex.Message);
        }
    }

    public async Task<Result<CourseDto>> CreateAsync(CreateCourseDto dto)
    {
        try
        {
            var course = new Course
            {
                Course_name = dto.Course_name,
                Course_id = Guid.NewGuid().ToString(),
                Instructor_id = dto.Instructor_id
            };

            await _unitOfWork.Courses.AddAsync(course);
            await _unitOfWork.SaveChangesAsync();

            return Result<CourseDto>.Success(new CourseDto
            {
                Course_id = course.Course_id,
                Course_name = course.Course_name,
                Instructor_id = course.Instructor_id,
                WorkWeight = course.WorkWeight,
                FirstExamWeight = course.FirstExamWeight,
                SecondExamWeight = course.SecondExamWeight,
                FinalExamWeight = course.FinalExamWeight
            });
        }
        catch (Exception ex)
        {
            return Result<CourseDto>.Failure(ex.Message);
        }
    }

    public async Task<Result<CourseDto>> UpdateAsync(string id, UpdateCourseDto dto)
    {
        try
        {
            var course = await _unitOfWork.Courses.GetByIdAsync(id);
            if (course == null)
                return Result<CourseDto>.Failure("Course not found");

            course.Course_name = dto.Course_name;
            course.Instructor_id = dto.Instructor_id ?? course.Instructor_id;
            await _unitOfWork.Courses.UpdateAsync(course);
            await _unitOfWork.SaveChangesAsync();

            return Result<CourseDto>.Success(new CourseDto
            {
                Course_id = course.Course_id,
                Course_name = course.Course_name,
                Instructor_id = course.Instructor_id,
                WorkWeight = course.WorkWeight,
                FirstExamWeight = course.FirstExamWeight,
                SecondExamWeight = course.SecondExamWeight,
                FinalExamWeight = course.FinalExamWeight
            });
        }
        catch (Exception ex)
        {
            return Result<CourseDto>.Failure(ex.Message);
        }
    }

    public async Task<Result> UpdateWeightsAsync(string id, CourseWeightsDto dto)
    {
        try
        {
            var course = await _unitOfWork.Courses.GetByIdAsync(id);
            if (course == null)
                return Result.Failure("Course not found");

            course.WorkWeight = dto.WorkWeight;
            course.FirstExamWeight = dto.FirstExamWeight;
            course.SecondExamWeight = dto.SecondExamWeight;
            course.FinalExamWeight = dto.FinalExamWeight;

            await _unitOfWork.Courses.UpdateAsync(course);
            await _unitOfWork.SaveChangesAsync();

            return Result.Success();
        }
        catch (Exception ex)
        {
            return Result.Failure(ex.Message);
        }
    }

    public async Task<Result> DeleteAsync(string id)
    {
        try
        {
            var course = await _unitOfWork.Courses.GetByIdAsync(id);
            if (course == null)
                return Result.Failure("Course not found");

            await _unitOfWork.Courses.DeleteAsync(course);
            await _unitOfWork.SaveChangesAsync();
            return Result.Success();
        }
        catch (Exception ex)
        {
            return Result.Failure(ex.Message);
        }
    }
}
