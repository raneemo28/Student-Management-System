using App.Application.DTOs;
using App.Application.Common;
using App.Application.Common.Events;
using App.Application.Interfaces;
using App.Application.Repositories;
using MediatR;

namespace App.Application.CQRS.HomeworkSubmissions.Commands;

public record SubmitHomeworkCommand(CreateHomeworkSubmissionDto Dto) : IRequest<Result<HomeworkSubmissionDto>>;

public class SubmitHomeworkCommandHandler : IRequestHandler<SubmitHomeworkCommand, Result<HomeworkSubmissionDto>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IFileStorageService _fileStorageService;

    public SubmitHomeworkCommandHandler(IUnitOfWork unitOfWork, IFileStorageService fileStorageService)
    {
        _unitOfWork = unitOfWork;
        _fileStorageService = fileStorageService;
    }

    public async Task<Result<HomeworkSubmissionDto>> Handle(SubmitHomeworkCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var dto = request.Dto;
            var existingSubmission = await _unitOfWork.HomeworkSubmissions.GetByStudentAndHomeworkAsync(dto.Student_id, dto.Homework_id);
            if (existingSubmission != null)
                return Result<HomeworkSubmissionDto>.Failure("Student has already submitted this homework");

            var fileUrl = await _fileStorageService.SaveFileAsync(new MemoryStream(dto.SolutionFileBytes), dto.SolutionFileName, "Pdf");

            var submission = new App.domain.entity.HomeworkSubmission
            {
                Homework_id = dto.Homework_id,
                Student_id = dto.Student_id,
                SolutionFileName = dto.SolutionFileName,
                SolutionFileUrl = fileUrl,
                SubmittedAt = DateTime.UtcNow,
                Submission_id = Guid.NewGuid().ToString()
            };

            await _unitOfWork.HomeworkSubmissions.AddAsync(submission);

            var domainEvent = new App.domain.entity.DomainEvent
            {
                Id = Guid.NewGuid(),
                EventType = "HomeworkSubmissionCreated",
                Data = DomainEventSerializer.Serialize(new { submission.Submission_id, submission.Homework_id, submission.Student_id, submission.SolutionFileName, submission.SolutionFileUrl, submission.SubmittedAt }),
                OccurredOn = DateTime.UtcNow,
                IsProcessed = false
            };

            await _unitOfWork.DomainEvents.AddAsync(domainEvent);
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
}
