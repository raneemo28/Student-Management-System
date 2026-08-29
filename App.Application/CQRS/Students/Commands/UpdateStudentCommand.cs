using App.Application.DTOs;
using App.Application.Common;
using App.Application.Common.Events;
using App.Application.Repositories;
using MediatR;

namespace App.Application.CQRS.Students.Commands;

public record UpdateStudentCommand(string Id, UpdateStudentDto Dto) : IRequest<Result<StudentDto>>;

public class UpdateStudentCommandHandler : IRequestHandler<UpdateStudentCommand, Result<StudentDto>>
{
    private readonly IUnitOfWork _unitOfWork;

    public UpdateStudentCommandHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<StudentDto>> Handle(UpdateStudentCommand request, CancellationToken cancellationToken)
    {
        var student = await _unitOfWork.Students.GetByIdAsync(request.Id);
        if (student == null)
            return Result<StudentDto>.Failure("Student not found");

        student.FirstName = request.Dto.FirstName;
        student.LastName = request.Dto.LastName;

        await _unitOfWork.Students.UpdateAsync(student);

        var domainEvent = new App.domain.entity.DomainEvent
        {
            Id = Guid.NewGuid(),
            EventType = "StudentUpdated",
            Data = DomainEventSerializer.Serialize(new { student.Student_id, student.FirstName, student.LastName }),
            OccurredOn = DateTime.UtcNow,
            IsProcessed = false
        };

        await _unitOfWork.DomainEvents.AddAsync(domainEvent);
        await _unitOfWork.SaveChangesAsync();

        return Result<StudentDto>.Success(new StudentDto
        {
            Student_id = student.Student_id,
            FirstName = student.FirstName,
            LastName = student.LastName,
            User_id = student.User_id
        });
    }
}
