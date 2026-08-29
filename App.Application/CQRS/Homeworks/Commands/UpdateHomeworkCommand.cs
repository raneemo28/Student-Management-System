using App.Application.DTOs;
using App.Application.Common;
using App.Application.Common.Events;
using App.Application.Interfaces;
using App.Application.Repositories;
using MediatR;

namespace App.Application.CQRS.Homeworks.Commands;

public record UpdateHomeworkCommand(string Id, UpdateHomeworkDto Dto) : IRequest<Result<HomeworkDto>>;

public class UpdateHomeworkCommandHandler : IRequestHandler<UpdateHomeworkCommand, Result<HomeworkDto>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IFileStorageService _fileStorageService;

    public UpdateHomeworkCommandHandler(IUnitOfWork unitOfWork, IFileStorageService fileStorageService)
    {
        _unitOfWork = unitOfWork;
        _fileStorageService = fileStorageService;
    }

    public async Task<Result<HomeworkDto>> Handle(UpdateHomeworkCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var homework = await _unitOfWork.Homeworks.GetByIdAsync(request.Id);
            if (homework == null)
                return Result<HomeworkDto>.Failure("Homework not found");

            homework.Title = request.Dto.Title;
            homework.Description = request.Dto.Description;
            homework.DueDate = request.Dto.DueDate;
            homework.TotalMarks = request.Dto.TotalMarks;

            if (request.Dto.QuestionFileBytes != null && request.Dto.QuestionFileBytes.Length > 0)
            {
                await _fileStorageService.DeleteFileAsync(homework.QuestionFileUrl);
                homework.QuestionFileUrl = await _fileStorageService.SaveFileAsync(new MemoryStream(request.Dto.QuestionFileBytes), request.Dto.QuestionFileName!, "Pdf");
                homework.QuestionFileName = request.Dto.QuestionFileName!;
            }

            await _unitOfWork.Homeworks.UpdateAsync(homework);

            var domainEvent = new App.domain.entity.DomainEvent
            {
                Id = Guid.NewGuid(),
                EventType = "HomeworkUpdated",
                Data = DomainEventSerializer.Serialize(new { homework.Homework_id, homework.Title, homework.Description, homework.DueDate, homework.TotalMarks, homework.QuestionFileName, homework.QuestionFileUrl }),
                OccurredOn = DateTime.UtcNow,
                IsProcessed = false
            };

            await _unitOfWork.DomainEvents.AddAsync(domainEvent);
            await _unitOfWork.SaveChangesAsync();

            return Result<HomeworkDto>.Success(new HomeworkDto
            {
                Homework_id = homework.Homework_id,
                Course_id = homework.Course_id,
                Title = homework.Title,
                Description = homework.Description,
                QuestionFileName = homework.QuestionFileName,
                QuestionFileUrl = homework.QuestionFileUrl,
                DueDate = homework.DueDate,
                TotalMarks = homework.TotalMarks,
                CreatedAt = homework.CreatedAt
            });
        }
        catch (Exception ex)
        {
            return Result<HomeworkDto>.Failure(ex.Message);
        }
    }
}
