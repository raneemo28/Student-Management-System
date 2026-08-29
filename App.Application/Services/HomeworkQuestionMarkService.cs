using App.Application.DTOs;
using App.Application.Interfaces;
using App.Application.Common;
using App.Application.Repositories;
using App.domain.entity;

namespace App.Application.Services;

public class HomeworkQuestionMarkService : IHomeworkQuestionMarkService
{
    private readonly IUnitOfWork _unitOfWork;

    public HomeworkQuestionMarkService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<IEnumerable<HomeworkQuestionMarkDto>>> GetBySubmissionAsync(string submissionId)
    {
        try
        {
            var questionMarks = await _unitOfWork.HomeworkQuestionMarks.GetBySubmissionIdAsync(submissionId);
            var questionMarkDtos = questionMarks.Select(qm => new HomeworkQuestionMarkDto
            {
                QuestionMark_id = qm.QuestionMark_id,
                Submission_id = qm.Submission_id,
                QuestionNumber = qm.QuestionNumber,
                QuestionDescription = qm.QuestionDescription,
                MaxMarks = qm.MaxMarks,
                ObtainedMarks = qm.ObtainedMarks
            });

            return Result<IEnumerable<HomeworkQuestionMarkDto>>.Success(questionMarkDtos);
        }
        catch (Exception ex)
        {
            return Result<IEnumerable<HomeworkQuestionMarkDto>>.Failure(ex.Message);
        }
    }

    public async Task<Result<HomeworkQuestionMarkDto>> CreateAsync(CreateHomeworkQuestionMarkDto dto)
    {
        try
        {
            var questionMark = new HomeworkQuestionMark
            {
                Submission_id = dto.Submission_id,
                QuestionNumber = dto.QuestionNumber,
                QuestionDescription = dto.QuestionDescription,
                MaxMarks = dto.MaxMarks,
                ObtainedMarks = dto.ObtainedMarks,
                QuestionMark_id = Guid.NewGuid().ToString()
            };

            await _unitOfWork.HomeworkQuestionMarks.AddAsync(questionMark);
            await _unitOfWork.SaveChangesAsync();

            return Result<HomeworkQuestionMarkDto>.Success(new HomeworkQuestionMarkDto
            {
                QuestionMark_id = questionMark.QuestionMark_id,
                Submission_id = questionMark.Submission_id,
                QuestionNumber = questionMark.QuestionNumber,
                QuestionDescription = questionMark.QuestionDescription,
                MaxMarks = questionMark.MaxMarks,
                ObtainedMarks = questionMark.ObtainedMarks
            });
        }
        catch (Exception ex)
        {
            return Result<HomeworkQuestionMarkDto>.Failure(ex.Message);
        }
    }

    public async Task<Result> DeleteAsync(string questionMarkId)
    {
        try
        {
            var questionMark = await _unitOfWork.HomeworkQuestionMarks.GetByIdAsync(questionMarkId);
            if (questionMark == null)
                return Result.Failure("Question mark not found");

            await _unitOfWork.HomeworkQuestionMarks.DeleteAsync(questionMark);
            await _unitOfWork.SaveChangesAsync();

            return Result.Success();
        }
        catch (Exception ex)
        {
            return Result.Failure(ex.Message);
        }
    }
}
