using App.Application.Common;
using App.Application.Common.Events;
using App.Application.Repositories;
using MediatR;

namespace App.Application.CQRS.HomeworkQuestionMarks.Commands;

public record DeleteHomeworkQuestionMarkCommand(string QuestionMarkId) : IRequest<Result>;

public class DeleteHomeworkQuestionMarkCommandHandler : IRequestHandler<DeleteHomeworkQuestionMarkCommand, Result>
{
    private readonly IUnitOfWork _unitOfWork;

    public DeleteHomeworkQuestionMarkCommandHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<Result> Handle(DeleteHomeworkQuestionMarkCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var questionMark = await _unitOfWork.HomeworkQuestionMarks.GetByIdAsync(request.QuestionMarkId);
            if (questionMark == null)
                return Result.Failure("Question mark not found");

            await _unitOfWork.HomeworkQuestionMarks.DeleteAsync(questionMark);

            var domainEvent = new App.domain.entity.DomainEvent
            {
                Id = Guid.NewGuid(),
                EventType = "HomeworkQuestionMarkDeleted",
                Data = DomainEventSerializer.Serialize(new { questionMark.QuestionMark_id }),
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
