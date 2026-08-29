using App.Application.Repositories;
using App.domain.entity;
using App.infra.Persistence;
using Microsoft.EntityFrameworkCore;

namespace App.infra.Repositories;

public class UnitOfWork : IUnitOfWork
{
    private readonly AppDbContext _context;
    private IStudentRepository? _students;
    private IInstructorRepository? _instructors;
    private IEmployeeRepository? _employees;
    private ICourseRepository? _courses;
    private ICourseStudentRepository? _courseStudents;
    private ICourseContentRepository? _courseContents;
    private IAdvertisementRepository? _advertisements;
    private IHomeworkRepository? _homeworks;
    private IHomeworkSubmissionRepository? _homeworkSubmissions;
    private IHomeworkSolutionRepository? _homeworkSolutions;
    private IHomeworkQuestionMarkRepository? _homeworkQuestionMarks;
    private IRepository<DomainEvent>? _domainEvents;

    public UnitOfWork(AppDbContext context)
    {
        _context = context;
    }

    public IStudentRepository Students => _students ??= new StudentRepository(_context);
    public IInstructorRepository Instructors => _instructors ??= new InstructorRepository(_context);
    public IEmployeeRepository Employees => _employees ??= new EmployeeRepository(_context);
    public ICourseRepository Courses => _courses ??= new CourseRepository(_context);
    public ICourseStudentRepository CourseStudents => _courseStudents ??= new CourseStudentRepository(_context);
    public ICourseContentRepository CourseContents => _courseContents ??= new CourseContentRepository(_context);
    public IAdvertisementRepository Advertisements => _advertisements ??= new AdvertisementRepository(_context);
    public IHomeworkRepository Homeworks => _homeworks ??= new HomeworkRepository(_context);
    public IHomeworkSubmissionRepository HomeworkSubmissions => _homeworkSubmissions ??= new HomeworkSubmissionRepository(_context);
    public IHomeworkSolutionRepository HomeworkSolutions => _homeworkSolutions ??= new HomeworkSolutionRepository(_context);
    public IHomeworkQuestionMarkRepository HomeworkQuestionMarks => _homeworkQuestionMarks ??= new HomeworkQuestionMarkRepository(_context);
    public IRepository<DomainEvent> DomainEvents => _domainEvents ??= new Repository<DomainEvent>(_context);

    public async Task<int> SaveChangesAsync()
    {
        return await _context.SaveChangesAsync();
    }

    public ValueTask DisposeAsync()
    {
        return _context.DisposeAsync();
    }
}
