using App.domain.ReadModels;
using Microsoft.EntityFrameworkCore;

namespace App.infra.Persistence;

public class AppReadDbContext : DbContext
{
    public AppReadDbContext(DbContextOptions<AppReadDbContext> options) : base(options) { }

    public DbSet<StudentRead> StudentsRead => Set<StudentRead>();
    public DbSet<EmployeeRead> EmployeesRead => Set<EmployeeRead>();
    public DbSet<InstructorRead> InstructorsRead => Set<InstructorRead>();
    public DbSet<CourseRead> CoursesRead => Set<CourseRead>();
    public DbSet<CourseStudentRead> EnrollmentsRead => Set<CourseStudentRead>();
    public DbSet<CourseContentRead> CourseContentsRead => Set<CourseContentRead>();
    public DbSet<AdvertisementRead> AdvertisementsRead => Set<AdvertisementRead>();
    public DbSet<HomeworkRead> HomeworksRead => Set<HomeworkRead>();
    public DbSet<HomeworkSubmissionRead> HomeworkSubmissionsRead => Set<HomeworkSubmissionRead>();
    public DbSet<HomeworkSolutionRead> HomeworkSolutionsRead => Set<HomeworkSolutionRead>();
    public DbSet<HomeworkQuestionMarkRead> HomeworkQuestionMarksRead => Set<HomeworkQuestionMarkRead>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<StudentRead>()
            .HasKey(s => s.Student_id);

        modelBuilder.Entity<InstructorRead>()
            .HasKey(i => i.Instructor_id);

        modelBuilder.Entity<EmployeeRead>()
            .HasKey(e => e.Employee_id);

        modelBuilder.Entity<CourseRead>()
            .HasKey(c => c.Course_id);

        modelBuilder.Entity<CourseStudentRead>()
            .HasKey(cs => new { cs.Student_id, cs.Course_id });

        modelBuilder.Entity<CourseContentRead>()
            .HasKey(cc => cc.Content_id);

        modelBuilder.Entity<AdvertisementRead>()
            .HasKey(a => a.Advertisement_id);

        modelBuilder.Entity<HomeworkRead>()
            .HasKey(h => h.Homework_id);

        modelBuilder.Entity<HomeworkSubmissionRead>()
            .HasKey(hs => hs.Submission_id);

        modelBuilder.Entity<HomeworkSolutionRead>()
            .HasKey(hs => hs.Solution_id);

        modelBuilder.Entity<HomeworkQuestionMarkRead>()
            .HasKey(hqm => hqm.QuestionMark_id);
    }
}
