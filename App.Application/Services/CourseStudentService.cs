using App.Application.DTOs;
using App.Application.Interfaces;
using App.Application.Common;
using App.Application.Repositories;
using App.domain.entity;

namespace App.Application.Services;

public class CourseStudentService : ICourseStudentService
{
    private readonly IUnitOfWork _unitOfWork;

    public CourseStudentService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<IEnumerable<CourseStudentDto>>> GetAllAsync()
    {
        try
        {
            var enrollments = await _unitOfWork.CourseStudents.GetAllAsync();
            var enrollmentDtos = enrollments.Select(cs => new CourseStudentDto
            {
                Student_id = cs.Student_id,
                Course_id = cs.Course_id,
                EnrolledAt = cs.EnrolledAt,
                WorkMark = cs.WorkMark,
                FirstExamMark = cs.FirstExamMark,
                SecondExamMark = cs.SecondExamMark,
                FinalMark = cs.FinalMark
            });

            return Result<IEnumerable<CourseStudentDto>>.Success(enrollmentDtos);
        }
        catch (Exception ex)
        {
            return Result<IEnumerable<CourseStudentDto>>.Failure(ex.Message);
        }
    }

    public async Task<Result<CourseStudentDto>> GetByIdAsync(string studentId, string courseId)
    {
        try
        {
            var enrollment = await _unitOfWork.CourseStudents.GetByIdAsync(studentId, courseId);
            if (enrollment == null)
                return Result<CourseStudentDto>.Failure("Enrollment not found");

            return Result<CourseStudentDto>.Success(new CourseStudentDto
            {
                Student_id = enrollment.Student_id,
                Course_id = enrollment.Course_id,
                EnrolledAt = enrollment.EnrolledAt,
                WorkMark = enrollment.WorkMark,
                FirstExamMark = enrollment.FirstExamMark,
                SecondExamMark = enrollment.SecondExamMark,
                FinalMark = enrollment.FinalMark
            });
        }
        catch (Exception ex)
        {
            return Result<CourseStudentDto>.Failure(ex.Message);
        }
    }

    public async Task<Result<CourseStudentDto>> CreateAsync(CreateCourseStudentDto dto)
    {
        try
        {
            var enrollment = new Course_student
            {
                Student_id = dto.Student_id,
                Course_id = dto.Course_id,
                EnrolledAt = DateTime.UtcNow
            };

            await _unitOfWork.CourseStudents.AddAsync(enrollment);
            await _unitOfWork.SaveChangesAsync();

            return Result<CourseStudentDto>.Success(new CourseStudentDto
            {
                Student_id = enrollment.Student_id,
                Course_id = enrollment.Course_id,
                EnrolledAt = enrollment.EnrolledAt,
                WorkMark = enrollment.WorkMark,
                FirstExamMark = enrollment.FirstExamMark,
                SecondExamMark = enrollment.SecondExamMark,
                FinalMark = enrollment.FinalMark
            });
        }
        catch (Exception ex)
        {
            return Result<CourseStudentDto>.Failure(ex.Message);
        }
    }

    public async Task<Result> DeleteAsync(string studentId, string courseId)
    {
        try
        {
            var enrollment = await _unitOfWork.CourseStudents.GetByIdAsync(studentId, courseId);
            if (enrollment == null)
                return Result.Failure("Enrollment not found");

            await _unitOfWork.CourseStudents.DeleteAsync(enrollment);
            await _unitOfWork.SaveChangesAsync();
            return Result.Success();
        }
        catch (Exception ex)
        {
            return Result.Failure(ex.Message);
        }
    }

    public async Task<Result<CourseStudentDto>> UpdateMarksAsync(string studentId, string courseId, UpdateMarksDto dto)
    {
        try
        {
            var enrollment = await _unitOfWork.CourseStudents.GetByIdAsync(studentId, courseId);
            if (enrollment == null)
                return Result<CourseStudentDto>.Failure("Enrollment not found");

            if (dto.WorkMark.HasValue) enrollment.WorkMark = dto.WorkMark.Value;
            if (dto.FirstExamMark.HasValue) enrollment.FirstExamMark = dto.FirstExamMark.Value;
            if (dto.SecondExamMark.HasValue) enrollment.SecondExamMark = dto.SecondExamMark.Value;

            await _unitOfWork.CourseStudents.UpdateAsync(enrollment);
            await _unitOfWork.SaveChangesAsync();

            return Result<CourseStudentDto>.Success(new CourseStudentDto
            {
                Student_id = enrollment.Student_id,
                Course_id = enrollment.Course_id,
                EnrolledAt = enrollment.EnrolledAt,
                WorkMark = enrollment.WorkMark,
                FirstExamMark = enrollment.FirstExamMark,
                SecondExamMark = enrollment.SecondExamMark,
                FinalMark = enrollment.FinalMark
            });
        }
        catch (Exception ex)
        {
            return Result<CourseStudentDto>.Failure(ex.Message);
        }
    }

    public async Task<Result<CourseStudentDto>> GetMarksAsync(string studentId, string courseId)
    {
        try
        {
            var enrollment = await _unitOfWork.CourseStudents.GetByIdAsync(studentId, courseId);
            if (enrollment == null)
                return Result<CourseStudentDto>.Failure("Enrollment not found");

            return Result<CourseStudentDto>.Success(new CourseStudentDto
            {
                Student_id = enrollment.Student_id,
                Course_id = enrollment.Course_id,
                EnrolledAt = enrollment.EnrolledAt,
                WorkMark = enrollment.WorkMark,
                FirstExamMark = enrollment.FirstExamMark,
                SecondExamMark = enrollment.SecondExamMark,
                FinalMark = enrollment.FinalMark
            });
        }
        catch (Exception ex)
        {
            return Result<CourseStudentDto>.Failure(ex.Message);
        }
    }
}
