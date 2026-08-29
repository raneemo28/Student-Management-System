using App.Application.Common;
using App.Application.Common.Events;
using App.Application.Repositories;
using MediatR;

namespace App.Application.CQRS.Homeworks.Commands;

public record DeleteHomeworkCommand(string Id) : IRequest<Result>;

public class DeleteHomeworkCommandHandler : IRequestHandler<DeleteHomeworkCommand, Result>
{
    private readonly IUnitOfWork _unitOfWork;

    public DeleteHomeworkCommandHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<Result> Handle(DeleteHomeworkCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var homework = await _unitOfWork.Homeworks.GetByIdAsync(request.Id);
            if (homework == null)
                return Result.Failure("Homework not found");

            await _unitOfWork.Homeworks.DeleteAsync(homework);

            var domainEvent = new App.domain.entity.DomainEvent
            {
                Id = Guid.NewGuid(),
                EventType = "HomeworkDeleted",
                Data = DomainEventSerializer.Serialize(new { homework.Homework_id }),
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
