using App.Application.DTOs;
using App.Application.Common;
using App.Application.Common.Events;
using App.Application.Repositories;
using MediatR;

namespace App.Application.CQRS.Instructors.Commands;

public record CreateInstructorCommand(CreateInstructorDto Dto) : IRequest<Result<InstructorDto>>;

public class CreateInstructorCommandHandler : IRequestHandler<CreateInstructorCommand, Result<InstructorDto>>
{
    private readonly IUnitOfWork _unitOfWork;

    public CreateInstructorCommandHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<InstructorDto>> Handle(CreateInstructorCommand request, CancellationToken cancellationToken)
    {
        var instructor = new App.domain.entity.Instructor
        {
            FirstName = request.Dto.FirstName,
            LastName = request.Dto.LastName,
            Instructor_id = Guid.NewGuid().ToString()
        };

        await _unitOfWork.Instructors.AddAsync(instructor);

        var domainEvent = new App.domain.entity.DomainEvent
        {
            Id = Guid.NewGuid(),
            EventType = "InstructorCreated",
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
