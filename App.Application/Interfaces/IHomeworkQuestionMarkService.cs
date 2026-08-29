using App.Application.DTOs;
using App.Application.Common;

namespace App.Application.Interfaces;

public interface IHomeworkQuestionMarkService
{
    Task<Result<IEnumerable<HomeworkQuestionMarkDto>>> GetBySubmissionAsync(string submissionId);
    Task<Result<HomeworkQuestionMarkDto>> CreateAsync(CreateHomeworkQuestionMarkDto dto);
    Task<Result> DeleteAsync(string questionMarkId);
}
