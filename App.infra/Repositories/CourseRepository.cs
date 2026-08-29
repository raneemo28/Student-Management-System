using App.Application.Repositories;
using App.domain.entity;
using App.infra.Persistence;
using Microsoft.EntityFrameworkCore;

namespace App.infra.Repositories;

public class CourseRepository : Repository<Course>, ICourseRepository
{
    public CourseRepository(AppDbContext context) : base(context) { }
}
