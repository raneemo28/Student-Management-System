using App.Application.DTOs;
using App.Application.Repositories;
using MediatR;

namespace App.Application.CQRS.Instructors.Queries;

public record GetInstructorByIdQuery(string Id) : IRequest<InstructorDto?>;

public class GetInstructorByIdQueryHandler : IRequestHandler<GetInstructorByIdQuery, InstructorDto?>
{
    private readonly IInstructorReadRepository _readRepository;

    public GetInstructorByIdQueryHandler(IInstructorReadRepository readRepository)
    {
        _readRepository = readRepository;
    }

    public async Task<InstructorDto?> Handle(GetInstructorByIdQuery request, CancellationToken cancellationToken)
    {
        var instructor = await _readRepository.GetByIdAsync(request.Id);
        if (instructor == null) return null;

        return new InstructorDto
        {
            Instructor_id = instructor.Instructor_id,
            FirstName = instructor.FirstName,
            LastName = instructor.LastName,
            User_id = instructor.User_id
        };
    }
}
