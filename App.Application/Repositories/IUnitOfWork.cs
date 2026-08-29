using App.Application.DTOs;
using App.Application.Common;
using App.domain.entity;

namespace App.Application.Repositories;

public interface IUnitOfWork : IAsyncDisposable
{
    IStudentRepository Students { get; }
    IInstructorRepository Instructors { get; }
    IEmployeeRepository Employees { get; }
    ICourseRepository Courses { get; }
    ICourseStudentRepository CourseStudents { get; }
    ICourseContentRepository CourseContents { get; }
    IAdvertisementRepository Advertisements { get; }
    IHomeworkRepository Homeworks { get; }
    IHomeworkSubmissionRepository HomeworkSubmissions { get; }
    IHomeworkSolutionRepository HomeworkSolutions { get; }
    IHomeworkQuestionMarkRepository HomeworkQuestionMarks { get; }
    IRepository<DomainEvent> DomainEvents { get; }
    Task<int> SaveChangesAsync();
}
