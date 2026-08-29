using App.Application.DTOs;
using App.Application.Repositories;
using MediatR;

namespace App.Application.CQRS.Students.Queries;

public record GetAllStudentsQuery() : IRequest<IEnumerable<StudentDto>>;

public class GetAllStudentsQueryHandler : IRequestHandler<GetAllStudentsQuery, IEnumerable<StudentDto>>
{
    private readonly IStudentReadRepository _readRepository;

    public GetAllStudentsQueryHandler(IStudentReadRepository readRepository)
    {
        _readRepository = readRepository;
    }

    public async Task<IEnumerable<StudentDto>> Handle(GetAllStudentsQuery request, CancellationToken cancellationToken)
    {
        var students = await _readRepository.GetAllAsync();
        return students.Select(s => new StudentDto
        {
            Student_id = s.Student_id,
            FirstName = s.FirstName,
            LastName = s.LastName,
            User_id = s.User_id
        });
    }
}
