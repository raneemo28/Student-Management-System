using App.Application.Common;
using App.Application.Common.Events;
using App.Application.Interfaces;
using App.Application.Repositories;
using MediatR;

namespace App.Application.CQRS.HomeworkSolutions.Commands;

public record DeleteHomeworkSolutionCommand(string SolutionId) : IRequest<Result>;

public class DeleteHomeworkSolutionCommandHandler : IRequestHandler<DeleteHomeworkSolutionCommand, Result>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IFileStorageService _fileStorageService;

    public DeleteHomeworkSolutionCommandHandler(IUnitOfWork unitOfWork, IFileStorageService fileStorageService)
    {
        _unitOfWork = unitOfWork;
        _fileStorageService = fileStorageService;
    }

    public async Task<Result> Handle(DeleteHomeworkSolutionCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var solution = await _unitOfWork.HomeworkSolutions.GetByIdAsync(request.SolutionId);
            if (solution == null)
                return Result.Failure("Solution not found");

            await _fileStorageService.DeleteFileAsync(solution.SolutionFileUrl);
            await _unitOfWork.HomeworkSolutions.DeleteAsync(solution);

            var domainEvent = new App.domain.entity.DomainEvent
            {
                Id = Guid.NewGuid(),
                EventType = "HomeworkSolutionDeleted",
                Data = DomainEventSerializer.Serialize(new { solution.Solution_id }),
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
