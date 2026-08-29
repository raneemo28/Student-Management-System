using App.Application.DTOs;
using App.Application.Common;

namespace App.Application.Interfaces;

public interface IHomeworkSubmissionService
{
    Task<Result<IEnumerable<HomeworkSubmissionDto>>> GetByHomeworkAsync(string homeworkId);
    Task<Result<HomeworkSubmissionDto>> GetByStudentAsync(string studentId, string homeworkId);
    Task<Result<HomeworkSubmissionDto>> SubmitAsync(CreateHomeworkSubmissionDto dto);
    Task<Result> DeleteAsync(string submissionId);
}
