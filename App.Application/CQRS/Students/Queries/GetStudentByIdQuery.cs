using App.Application.DTOs;
using App.Application.Repositories;
using MediatR;

namespace App.Application.CQRS.Students.Queries;

public record GetStudentByIdQuery(string Id) : IRequest<StudentDto?>;

public class GetStudentByIdQueryHandler : IRequestHandler<GetStudentByIdQuery, StudentDto?>
{
    private readonly IStudentReadRepository _readRepository;

    public GetStudentByIdQueryHandler(IStudentReadRepository readRepository)
    {
        _readRepository = readRepository;
    }

    public async Task<StudentDto?> Handle(GetStudentByIdQuery request, CancellationToken cancellationToken)
    {
        var student = await _readRepository.GetByIdAsync(request.Id);
        if (student == null) return null;

        return new StudentDto
        {
            Student_id = student.Student_id,
            FirstName = student.FirstName,
            LastName = student.LastName,
            User_id = student.User_id
        };
    }
}
