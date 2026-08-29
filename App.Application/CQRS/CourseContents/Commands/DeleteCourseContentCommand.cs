using App.Application.Common;
using App.Application.Common.Events;
using App.Application.Interfaces;
using App.Application.Repositories;
using MediatR;

namespace App.Application.CQRS.CourseContents.Commands;

public record DeleteCourseContentCommand(string Id) : IRequest<Result>;

public class DeleteCourseContentCommandHandler : IRequestHandler<DeleteCourseContentCommand, Result>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IFileStorageService _fileStorageService;

    public DeleteCourseContentCommandHandler(IUnitOfWork unitOfWork, IFileStorageService fileStorageService)
    {
        _unitOfWork = unitOfWork;
        _fileStorageService = fileStorageService;
    }

    public async Task<Result> Handle(DeleteCourseContentCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var courseContent = await _unitOfWork.CourseContents.GetByIdAsync(request.Id);
            if (courseContent == null)
                return Result.Failure("Course content not found");

            await _fileStorageService.DeleteFileAsync(courseContent.FileUrl);
            await _unitOfWork.CourseContents.DeleteAsync(courseContent);

            var domainEvent = new App.domain.entity.DomainEvent
            {
                Id = Guid.NewGuid(),
                EventType = "CourseContentDeleted",
                Data = DomainEventSerializer.Serialize(new { courseContent.Content_id }),
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
