using App.Application.Repositories;
using App.domain.entity;
using App.infra.Persistence;
using Microsoft.EntityFrameworkCore;

namespace App.infra.Repositories;

public class InstructorRepository : Repository<Instructor>, IInstructorRepository
{
    public InstructorRepository(AppDbContext context) : base(context) { }
}
