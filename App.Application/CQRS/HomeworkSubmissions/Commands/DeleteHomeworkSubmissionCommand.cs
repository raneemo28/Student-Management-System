using App.Application.Common;
using App.Application.Common.Events;
using App.Application.Repositories;
using App.Application.Interfaces;
using MediatR;

namespace App.Application.CQRS.HomeworkSubmissions.Commands;

public record DeleteHomeworkSubmissionCommand(string SubmissionId) : IRequest<Result>;

public class DeleteHomeworkSubmissionCommandHandler : IRequestHandler<DeleteHomeworkSubmissionCommand, Result>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IFileStorageService _fileStorageService;

    public DeleteHomeworkSubmissionCommandHandler(IUnitOfWork unitOfWork, IFileStorageService fileStorageService)
    {
        _unitOfWork = unitOfWork;
        _fileStorageService = fileStorageService;
    }

    public async Task<Result> Handle(DeleteHomeworkSubmissionCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var submission = await _unitOfWork.HomeworkSubmissions.GetByIdAsync(request.SubmissionId);
            if (submission == null)
                return Result.Failure("Submission not found");

            await _fileStorageService.DeleteFileAsync(submission.SolutionFileUrl);
            await _unitOfWork.HomeworkSubmissions.DeleteAsync(submission);

            var domainEvent = new App.domain.entity.DomainEvent
            {
                Id = Guid.NewGuid(),
                EventType = "HomeworkSubmissionDeleted",
                Data = DomainEventSerializer.Serialize(new { submission.Submission_id }),
                OccurredOn = DateTime.UtcNow,
                IsProcessed = false
            };

            await _unitOfWork.DomainEvents.AddAsync(domainEvent);
            await _unitOfWork.SaveChangesAsync();

            return Result.Success();
        }
        catch (Exception ex)
        {
            return Result.Failure(ex.Message);
        }
    }
}
