using App.Application.DTOs;
using App.Application.Repositories;
using MediatR;

namespace App.Application.CQRS.Employees.Queries;

public record GetEmployeeByIdQuery(string Id) : IRequest<EmployeeDto?>;

public class GetEmployeeByIdQueryHandler : IRequestHandler<GetEmployeeByIdQuery, EmployeeDto?>
{
    private readonly IEmployeeReadRepository _readRepository;

    public GetEmployeeByIdQueryHandler(IEmployeeReadRepository readRepository)
    {
        _readRepository = readRepository;
    }

    public async Task<EmployeeDto?> Handle(GetEmployeeByIdQuery request, CancellationToken cancellationToken)
    {
        var employee = await _readRepository.GetByIdAsync(request.Id);
        if (employee == null) return null;

        return new EmployeeDto
        {
            Employee_id = employee.Employee_id,
            FirstName = employee.FirstName,
            LastName = employee.LastName,
            User_id = employee.User_id
        };
    }
}
