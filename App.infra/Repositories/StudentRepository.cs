using App.Application.Repositories;
using App.domain.entity;
using App.infra.Persistence;
using Microsoft.EntityFrameworkCore;

namespace App.infra.Repositories;

public class StudentRepository : Repository<Student>, IStudentRepository
{
    public StudentRepository(AppDbContext context) : base(context) { }
}
