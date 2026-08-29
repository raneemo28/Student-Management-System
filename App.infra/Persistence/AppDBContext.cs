using App.domain.entity;
using Microsoft.EntityFrameworkCore;
using System.Reflection.Emit;

namespace App.infra.Persistence;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<Student> Students => Set<Student>();
    public DbSet<Instructor> Instructors => Set<Instructor>();
    public DbSet<Employee> Employees => Set<Employee>();
    public DbSet<Course> Courses => Set<Course>();
    public DbSet<Course_student> CourseStudents => Set<Course_student>();
    public DbSet<CourseContent> CourseContents => Set<CourseContent>();
    public DbSet<Advertisement> Advertisements => Set<Advertisement>();
    public DbSet<Homework> Homeworks => Set<Homework>();
    public DbSet<HomeworkSubmission> HomeworkSubmissions => Set<HomeworkSubmission>();
    public DbSet<HomeworkSolution> HomeworkSolutions => Set<HomeworkSolution>();
    public DbSet<HomeworkQuestionMark> HomeworkQuestionMarks => Set<HomeworkQuestionMark>();
    public DbSet<DomainEvent> DomainEvents => Set<DomainEvent>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Student>()
            .HasKey(s => s.Student_id);

        modelBuilder.Entity<Course>()
            .HasKey(c => c.Course_id);

        modelBuilder.Entity<Course_student>()
            .HasKey(sc => new { sc.Student_id, sc.Course_id });

        modelBuilder.Entity<Course_student>()
            .HasOne<Student>()
            .WithMany() 
            .HasForeignKey(sc => sc.Student_id)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<Course_student>()
            .HasOne<Course>()
            .WithMany() 
            .HasForeignKey(sc => sc.Course_id)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<Instructor>()
            .HasKey(i => i.Instructor_id);

        modelBuilder.Entity<Course>()
            .HasOne<Instructor>()
            .WithMany()
            .HasForeignKey(c => c.Instructor_id)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<CourseContent>()
            .HasKey(cc => cc.Content_id);

        modelBuilder.Entity<CourseContent>()
            .HasOne<Course>()
            .WithMany()
            .HasForeignKey(cc => cc.Course_id)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<Employee>()
            .HasKey(e => e.Employee_id);

        modelBuilder.Entity<Advertisement>()
            .HasKey(a => a.Advertisement_id);

        modelBuilder.Entity<Homework>()
            .HasKey(h => h.Homework_id);

        modelBuilder.Entity<Homework>()
            .HasOne<Course>()
            .WithMany()
            .HasForeignKey(h => h.Course_id)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<HomeworkSubmission>()
            .HasKey(hs => hs.Submission_id);

        modelBuilder.Entity<HomeworkSubmission>()
            .HasOne<Homework>()
            .WithMany()
            .HasForeignKey(hs => hs.Homework_id)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<HomeworkSubmission>()
            .HasOne<Student>()
            .WithMany()
            .HasForeignKey(hs => hs.Student_id)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<HomeworkSolution>()
            .HasKey(hs => hs.Solution_id);

        modelBuilder.Entity<HomeworkSolution>()
            .HasOne<Homework>()
            .WithMany()
            .HasForeignKey(hs => hs.Homework_id)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<HomeworkQuestionMark>()
            .HasKey(hqm => hqm.QuestionMark_id);

        modelBuilder.Entity<HomeworkQuestionMark>()
            .HasOne<HomeworkSubmission>()
            .WithMany()
            .HasForeignKey(hqm => hqm.Submission_id)
            .OnDelete(DeleteBehavior.Cascade);
    }
}