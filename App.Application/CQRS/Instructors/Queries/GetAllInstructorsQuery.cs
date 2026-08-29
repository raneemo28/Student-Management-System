using App.Application.DTOs;
using App.Application.Repositories;
using MediatR;

namespace App.Application.CQRS.Instructors.Queries;

public record GetAllInstructorsQuery() : IRequest<IEnumerable<InstructorDto>>;

public class GetAllInstructorsQueryHandler : IRequestHandler<GetAllInstructorsQuery, IEnumerable<InstructorDto>>
{
    private readonly IInstructorReadRepository _readRepository;

    public GetAllInstructorsQueryHandler(IInstructorReadRepository readRepository)
    {
        _readRepository = readRepository;
    }

    public async Task<IEnumerable<InstructorDto>> Handle(GetAllInstructorsQuery request, CancellationToken cancellationToken)
    {
        var instructors = await _readRepository.GetAllAsync();
        return instructors.Select(i => new InstructorDto
        {
            Instructor_id = i.Instructor_id,
            FirstName = i.FirstName,
            LastName = i.LastName,
            User_id = i.User_id
        });
    }
}
