using App.Application.DTOs;
using App.Application.Interfaces;
using App.Application.Common;
using App.Application.Repositories;
using App.domain.entity;

namespace App.Application.Services;

public class HomeworkSubmissionService : IHomeworkSubmissionService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IFileStorageService _fileStorageService;

    public HomeworkSubmissionService(IUnitOfWork unitOfWork, IFileStorageService fileStorageService)
    {
        _unitOfWork = unitOfWork;
        _fileStorageService = fileStorageService;
    }

    public async Task<Result<IEnumerable<HomeworkSubmissionDto>>> GetByHomeworkAsync(string homeworkId)
    {
        try
        {
            var submissions = await _unitOfWork.HomeworkSubmissions.GetByHomeworkIdAsync(homeworkId);
            var submissionDtos = submissions.Select(s => new HomeworkSubmissionDto
            {
                Submission_id = s.Submission_id,
                Homework_id = s.Homework_id,
                Student_id = s.Student_id,
                SolutionFileName = s.SolutionFileName,
                SolutionFileUrl = s.SolutionFileUrl,
                SubmittedAt = s.SubmittedAt,
                Mark = s.Mark
            });

            return Result<IEnumerable<HomeworkSubmissionDto>>.Success(submissionDtos);
        }
        catch (Exception ex)
        {
            return Result<IEnumerable<HomeworkSubmissionDto>>.Failure(ex.Message);
        }
    }

    public async Task<Result<HomeworkSubmissionDto>> GetByStudentAsync(string studentId, string homeworkId)
    {
        try
        {
            var submission = await _unitOfWork.HomeworkSubmissions.GetByStudentAndHomeworkAsync(studentId, homeworkId);
            if (submission == null)
                return Result<HomeworkSubmissionDto>.Failure("Submission not found");

            return Result<HomeworkSubmissionDto>.Success(new HomeworkSubmissionDto
            {
                Submission_id = submission.Submission_id,
                Homework_id = submission.Homework_id,
                Student_id = submission.Student_id,
                SolutionFileName = submission.SolutionFileName,
                SolutionFileUrl = submission.SolutionFileUrl,
                SubmittedAt = submission.SubmittedAt,
                Mark = submission.Mark
            });
        }
        catch (Exception ex)
        {
            return Result<HomeworkSubmissionDto>.Failure(ex.Message);
        }
    }

    public async Task<Result<HomeworkSubmissionDto>> SubmitAsync(CreateHomeworkSubmissionDto dto)
    {
        try
        {
            var existingSubmission = await _unitOfWork.HomeworkSubmissions.GetByStudentAndHomeworkAsync(dto.Student_id, dto.Homework_id);
            if (existingSubmission != null)
                return Result<HomeworkSubmissionDto>.Failure("Student has already submitted this homework");

            var fileUrl = await _fileStorageService.SaveFileAsync(
                new MemoryStream(dto.SolutionFileBytes),
                dto.SolutionFileName,
                "Pdf");

            var submission = new HomeworkSubmission
            {
                Homework_id = dto.Homework_id,
                Student_id = dto.Student_id,
                SolutionFileName = dto.SolutionFileName,
                SolutionFileUrl = fileUrl,
                SubmittedAt = DateTime.UtcNow,
                Submission_id = Guid.NewGuid().ToString()
            };

            await _unitOfWork.HomeworkSubmissions.AddAsync(submission);
            await _unitOfWork.SaveChangesAsync();

            return Result<HomeworkSubmissionDto>.Success(new HomeworkSubmissionDto
            {
                Submission_id = submission.Submission_id,
                Homework_id = submission.Homework_id,
                Student_id = submission.Student_id,
                SolutionFileName = submission.SolutionFileName,
                SolutionFileUrl = submission.SolutionFileUrl,
                SubmittedAt = submission.SubmittedAt,
                Mark = submission.Mark
            });
        }
        catch (Exception ex)
        {
            return Result<HomeworkSubmissionDto>.Failure(ex.Message);
        }
    }

    public async Task<Result> DeleteAsync(string submissionId)
    {
        try
        {
            var submission = await _unitOfWork.HomeworkSubmissions.GetByIdAsync(submissionId);
            if (submission == null)
                return Result.Failure("Submission not found");

            await _fileStorageService.DeleteFileAsync(submission.SolutionFileUrl);
            await _unitOfWork.HomeworkSubmissions.DeleteAsync(submission);
            await _unitOfWork.SaveChangesAsync();

            return Result.Success();
        }
        catch (Exception ex)
        {
            return Result.Failure(ex.Message);
        }
    }
}
