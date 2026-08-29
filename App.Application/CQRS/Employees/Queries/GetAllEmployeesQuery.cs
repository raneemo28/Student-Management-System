using App.Application.DTOs;
using App.Application.Repositories;
using MediatR;

namespace App.Application.CQRS.Employees.Queries;

public record GetAllEmployeesQuery() : IRequest<IEnumerable<EmployeeDto>>;

public class GetAllEmployeesQueryHandler : IRequestHandler<GetAllEmployeesQuery, IEnumerable<EmployeeDto>>
{
    private readonly IEmployeeReadRepository _readRepository;

    public GetAllEmployeesQueryHandler(IEmployeeReadRepository readRepository)
    {
        _readRepository = readRepository;
    }

    public async Task<IEnumerable<EmployeeDto>> Handle(GetAllEmployeesQuery request, CancellationToken cancellationToken)
    {
        var employees = await _readRepository.GetAllAsync();
        return employees.Select(e => new EmployeeDto
        {
            Employee_id = e.Employee_id,
            FirstName = e.FirstName,
            LastName = e.LastName,
            User_id = e.User_id
        });
    }
}
