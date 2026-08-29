using App.Application.DTOs;
using App.Application.Common;
using App.Application.Common.Events;
using App.Application.Interfaces;
using App.Application.Repositories;
using MediatR;

namespace App.Application.CQRS.Homeworks.Commands;

public record CreateHomeworkCommand(string CourseId, CreateHomeworkDto Dto) : IRequest<Result<HomeworkDto>>;

public class CreateHomeworkCommandHandler : IRequestHandler<CreateHomeworkCommand, Result<HomeworkDto>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IFileStorageService _fileStorageService;

    public CreateHomeworkCommandHandler(IUnitOfWork unitOfWork, IFileStorageService fileStorageService)
    {
        _unitOfWork = unitOfWork;
        _fileStorageService = fileStorageService;
    }

    public async Task<Result<HomeworkDto>> Handle(CreateHomeworkCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var dto = request.Dto;
            var fileUrl = await _fileStorageService.SaveFileAsync(new MemoryStream(dto.QuestionFileBytes), dto.QuestionFileName, "Pdf");

            var homework = new App.domain.entity.Homework
            {
                Course_id = request.CourseId,
                Title = dto.Title,
                Description = dto.Description,
                DueDate = dto.DueDate,
                TotalMarks = dto.TotalMarks,
                QuestionFileName = dto.QuestionFileName,
                QuestionFileUrl = fileUrl,
                Homework_id = Guid.NewGuid().ToString(),
                CreatedAt = DateTime.UtcNow
            };

            await _unitOfWork.Homeworks.AddAsync(homework);

            var domainEvent = new App.domain.entity.DomainEvent
            {
                Id = Guid.NewGuid(),
                EventType = "HomeworkCreated",
                Data = DomainEventSerializer.Serialize(new { homework.Homework_id, homework.Course_id, homework.Title, homework.Description, homework.DueDate, homework.TotalMarks, homework.QuestionFileName, homework.QuestionFileUrl, homework.CreatedAt }),
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
