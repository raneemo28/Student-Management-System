using App.Application.DTOs;
using App.Application.Common;
using App.Application.Common.Events;
using App.Application.Repositories;
using MediatR;

namespace App.Application.CQRS.HomeworkQuestionMarks.Commands;

public record CreateHomeworkQuestionMarkCommand(CreateHomeworkQuestionMarkDto Dto) : IRequest<Result<HomeworkQuestionMarkDto>>;

public class CreateHomeworkQuestionMarkCommandHandler : IRequestHandler<CreateHomeworkQuestionMarkCommand, Result<HomeworkQuestionMarkDto>>
{
    private readonly IUnitOfWork _unitOfWork;

    public CreateHomeworkQuestionMarkCommandHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<HomeworkQuestionMarkDto>> Handle(CreateHomeworkQuestionMarkCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var dto = request.Dto;
            var questionMark = new App.domain.entity.HomeworkQuestionMark
            {
                Submission_id = dto.Submission_id,
                QuestionNumber = dto.QuestionNumber,
                QuestionDescription = dto.QuestionDescription,
                MaxMarks = dto.MaxMarks,
                ObtainedMarks = dto.ObtainedMarks,
                QuestionMark_id = Guid.NewGuid().ToString()
            };

            await _unitOfWork.HomeworkQuestionMarks.AddAsync(questionMark);

            var domainEvent = new App.domain.entity.DomainEvent
            {
                Id = Guid.NewGuid(),
                EventType = "HomeworkQuestionMarkCreated",
                Data = DomainEventSerializer.Serialize(new { questionMark.QuestionMark_id, questionMark.Submission_id, questionMark.QuestionNumber, questionMark.QuestionDescription, questionMark.MaxMarks, questionMark.ObtainedMarks }),
                OccurredOn = DateTime.UtcNow,
                IsProcessed = false
            };

            await _unitOfWork.DomainEvents.AddAsync(domainEvent);
            await _unitOfWork.SaveChangesAsync();

            return Result<HomeworkQuestionMarkDto>.Success(new HomeworkQuestionMarkDto
            {
                QuestionMark_id = questionMark.QuestionMark_id,
                Submission_id = questionMark.Submission_id,
                QuestionNumber = questionMark.QuestionNumber,
                QuestionDescription = questionMark.QuestionDescription,
                MaxMarks = questionMark.MaxMarks,
                ObtainedMarks = questionMark.ObtainedMarks
            });
        }
        catch (Exception ex)
        {
            return Result<HomeworkQuestionMarkDto>.Failure(ex.Message);
        }
    }
}
