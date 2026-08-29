using App.Application.DTOs;
using App.Application.Common;
using App.Application.Common.Events;
using App.Application.Interfaces;
using App.Application.Repositories;
using MediatR;

namespace App.Application.CQRS.CourseContents.Commands;

public record UpdateCourseContentCommand(string Id, UpdateCourseContentDto Dto) : IRequest<Result<CourseContentDto>>;

public class UpdateCourseContentCommandHandler : IRequestHandler<UpdateCourseContentCommand, Result<CourseContentDto>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IFileStorageService _fileStorageService;

    public UpdateCourseContentCommandHandler(IUnitOfWork unitOfWork, IFileStorageService fileStorageService)
    {
        _unitOfWork = unitOfWork;
        _fileStorageService = fileStorageService;
    }

    public async Task<Result<CourseContentDto>> Handle(UpdateCourseContentCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var courseContent = await _unitOfWork.CourseContents.GetByIdAsync(request.Id);
            if (courseContent == null)
                return Result<CourseContentDto>.Failure("Course content not found");

            courseContent.Title = request.Dto.Title;
            courseContent.Description = request.Dto.Description;
            courseContent.ContentType = request.Dto.ContentType;
            courseContent.UpdatedAt = DateTime.UtcNow;

            if (request.Dto.FileBytes != null && request.Dto.FileBytes.Length > 0)
            {
                await _fileStorageService.DeleteFileAsync(courseContent.FileUrl);
                courseContent.FileUrl = await _fileStorageService.SaveFileAsync(new MemoryStream(request.Dto.FileBytes), request.Dto.FileName!, request.Dto.ContentType);
                courseContent.FileName = request.Dto.FileName!;
                courseContent.FileSize = request.Dto.FileSize ?? 0;
            }

            await _unitOfWork.CourseContents.UpdateAsync(courseContent);

            var domainEvent = new App.domain.entity.DomainEvent
            {
                Id = Guid.NewGuid(),
                EventType = "CourseContentUpdated",
                Data = DomainEventSerializer.Serialize(new { courseContent.Content_id, courseContent.Title, courseContent.Description, courseContent.ContentType, courseContent.FileName, courseContent.FileUrl, courseContent.FileSize, courseContent.UpdatedAt }),
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
