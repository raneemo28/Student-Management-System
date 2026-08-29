using App.Application.Repositories;
using App.domain.entity;
using App.infra.Persistence;
using Microsoft.EntityFrameworkCore;

namespace App.infra.Repositories;

public class EmployeeRepository : Repository<Employee>, IEmployeeRepository
{
    public EmployeeRepository(AppDbContext context) : base(context) { }
}
