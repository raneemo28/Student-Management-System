using App.Application.DTOs;
using App.Application.Common;
using App.Application.Common.Events;
using App.Application.Interfaces;
using App.Application.Repositories;
using MediatR;

namespace App.Application.CQRS.CourseContents.Commands;

public record CreateCourseContentCommand(string CourseId, CreateCourseContentDto Dto) : IRequest<Result<CourseContentDto>>;

public class CreateCourseContentCommandHandler : IRequestHandler<CreateCourseContentCommand, Result<CourseContentDto>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IFileStorageService _fileStorageService;

    public CreateCourseContentCommandHandler(IUnitOfWork unitOfWork, IFileStorageService fileStorageService)
    {
        _unitOfWork = unitOfWork;
        _fileStorageService = fileStorageService;
    }

    public async Task<Result<CourseContentDto>> Handle(CreateCourseContentCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var dto = request.Dto;
            var fileUrl = await _fileStorageService.SaveFileAsync(new MemoryStream(dto.FileBytes), dto.FileName, dto.ContentType);

            var courseContent = new App.domain.entity.CourseContent
            {
                Course_id = request.CourseId,
                Title = dto.Title,
                Description = dto.Description,
                ContentType = dto.ContentType,
                FileName = dto.FileName,
                FileUrl = fileUrl,
                FileSize = dto.FileSize,
                Content_id = Guid.NewGuid().ToString(),
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            await _unitOfWork.CourseContents.AddAsync(courseContent);

            var domainEvent = new App.domain.entity.DomainEvent
            {
                Id = Guid.NewGuid(),
                EventType = "CourseContentCreated",
                Data = DomainEventSerializer.Serialize(new { courseContent.Content_id, courseContent.Course_id, courseContent.Title, courseContent.Description, courseContent.ContentType, courseContent.FileName, courseContent.FileUrl, courseContent.FileSize, courseContent.CreatedAt }),
                OccurredOn = DateTime.UtcNow,
                IsProcessed = false
            };

            await _unitOfWork.DomainEvents.AddAsync(domainEvent);
            await _unitOfWork.SaveChangesAsync();

            return Result<CourseContentDto>.Success(new CourseContentDto
            {
                Content_id = courseContent.Content_id,
                Course_id = courseContent.Course_id,
                Title = courseContent.Title,
                Description = courseContent.Description,
                ContentType = courseContent.ContentType,
                FileName = courseContent.FileName,
                FileUrl = courseContent.FileUrl,
                FileSize = courseContent.FileSize,
                CreatedAt = courseContent.CreatedAt
            });
        }
        catch (Exception ex)
        {
            return Result<CourseContentDto>.Failure(ex.Message);
        }
    }
}
