using App.Application.DTOs;
using App.Application.Common;
using App.Application.Common.Events;
using App.Application.Interfaces;
using App.Application.Repositories;
using MediatR;

namespace App.Application.CQRS.HomeworkSolutions.Commands;

public record CreateHomeworkSolutionCommand(CreateHomeworkSolutionDto Dto) : IRequest<Result<HomeworkSolutionDto>>;

public class CreateHomeworkSolutionCommandHandler : IRequestHandler<CreateHomeworkSolutionCommand, Result<HomeworkSolutionDto>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IFileStorageService _fileStorageService;

    public CreateHomeworkSolutionCommandHandler(IUnitOfWork unitOfWork, IFileStorageService fileStorageService)
    {
        _unitOfWork = unitOfWork;
        _fileStorageService = fileStorageService;
    }

    public async Task<Result<HomeworkSolutionDto>> Handle(CreateHomeworkSolutionCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var dto = request.Dto;
            var fileUrl = await _fileStorageService.SaveFileAsync(new MemoryStream(dto.SolutionFileBytes), dto.SolutionFileName, "Pdf");

            var solution = new App.domain.entity.HomeworkSolution
            {
                Homework_id = dto.Homework_id,
                SolutionFileName = dto.SolutionFileName,
                SolutionFileUrl = fileUrl,
                Notes = dto.Notes,
                UploadedAt = DateTime.UtcNow,
                Solution_id = Guid.NewGuid().ToString()
            };

            await _unitOfWork.HomeworkSolutions.AddAsync(solution);

            var domainEvent = new App.domain.entity.DomainEvent
            {
                Id = Guid.NewGuid(),
                EventType = "HomeworkSolutionCreated",
                Data = DomainEventSerializer.Serialize(new { solution.Solution_id, solution.Homework_id, solution.SolutionFileName, solution.SolutionFileUrl, solution.Notes, solution.UploadedAt }),
                OccurredOn = DateTime.UtcNow,
                IsProcessed = false
            };

            await _unitOfWork.DomainEvents.AddAsync(domainEvent);
            await _unitOfWork.SaveChangesAsync();

            return Result<HomeworkSolutionDto>.Success(new HomeworkSolutionDto
            {
                Solution_id = solution.Solution_id,
                Homework_id = solution.Homework_id,
                SolutionFileName = solution.SolutionFileName,
                SolutionFileUrl = solution.SolutionFileUrl,
                UploadedAt = solution.UploadedAt,
                Notes = solution.Notes
            });
        }
        catch (Exception ex)
        {
            return Result<HomeworkSolutionDto>.Failure(ex.Message);
        }
    }
}
