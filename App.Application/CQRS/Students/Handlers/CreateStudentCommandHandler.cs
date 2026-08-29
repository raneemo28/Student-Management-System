using App.Application.DTOs;
using App.Application.Common;
using App.Application.Common.Events;
using App.Application.Repositories;
using MediatR;

namespace App.Application.CQRS.Students.Commands;

public class CreateStudentCommandHandler : IRequestHandler<CreateStudentCommand, Result<StudentDto>>
{
    private readonly IUnitOfWork _unitOfWork;

    public CreateStudentCommandHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<StudentDto>> Handle(CreateStudentCommand request, CancellationToken cancellationToken)
    {
        var student = new App.domain.entity.Student
        {
            FirstName = request.Dto.FirstName,
            LastName = request.Dto.LastName,
            Student_id = Guid.NewGuid().ToString()
        };

        await _unitOfWork.Students.AddAsync(student);

        var domainEvent = new App.domain.entity.DomainEvent
        {
            Id = Guid.NewGuid(),
            EventType = "StudentCreated",
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
