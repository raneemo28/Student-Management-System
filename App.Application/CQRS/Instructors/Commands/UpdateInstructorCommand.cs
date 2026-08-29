using App.Application.DTOs;
using App.Application.Common;
using App.Application.Common.Events;
using App.Application.Repositories;
using MediatR;

namespace App.Application.CQRS.Instructors.Commands;

public record UpdateInstructorCommand(string Id, UpdateInstructorDto Dto) : IRequest<Result<InstructorDto>>;

public class UpdateInstructorCommandHandler : IRequestHandler<UpdateInstructorCommand, Result<InstructorDto>>
{
    private readonly IUnitOfWork _unitOfWork;

    public UpdateInstructorCommandHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<InstructorDto>> Handle(UpdateInstructorCommand request, CancellationToken cancellationToken)
    {
        var instructor = await _unitOfWork.Instructors.GetByIdAsync(request.Id);
        if (instructor == null)
            return Result<InstructorDto>.Failure("Instructor not found");

        instructor.FirstName = request.Dto.FirstName;
        instructor.LastName = request.Dto.LastName;

        await _unitOfWork.Instructors.UpdateAsync(instructor);

        var domainEvent = new App.domain.entity.DomainEvent
        {
            Id = Guid.NewGuid(),
            EventType = "InstructorUpdated",
            Data = DomainEventSerializer.Serialize(new { instructor.Instructor_id, instructor.FirstName, instructor.LastName }),
            OccurredOn = DateTime.UtcNow,
            IsProcessed = false
        };

        await _unitOfWork.DomainEvents.AddAsync(domainEvent);
        await _unitOfWork.SaveChangesAsync();

        return Result<InstructorDto>.Success(new InstructorDto
        {
            Instructor_id = instructor.Instructor_id,
            FirstName = instructor.FirstName,
            LastName = instructor.LastName,
            User_id = instructor.User_id
        });
    }
}
