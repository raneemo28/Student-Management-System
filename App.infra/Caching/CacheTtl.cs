namespace App.infra.Caching;

public static class CacheTtl
{
    public static readonly TimeSpan StudentRead = TimeSpan.FromSeconds(30);
    public static readonly TimeSpan InstructorRead = TimeSpan.FromSeconds(60);
    public static readonly TimeSpan EmployeeRead = TimeSpan.FromSeconds(60);
    public static readonly TimeSpan CourseRead = TimeSpan.FromSeconds(60);
    public static readonly TimeSpan CourseStudentRead = TimeSpan.FromSeconds(30);
    public static readonly TimeSpan CourseContentRead = TimeSpan.FromSeconds(120);
    public static readonly TimeSpan AdvertisementRead = TimeSpan.FromSeconds(300);
    public static readonly TimeSpan HomeworkRead = TimeSpan.FromSeconds(15);
    public static readonly TimeSpan HomeworkSubmissionRead = TimeSpan.FromSeconds(15);
    public static readonly TimeSpan HomeworkSolutionRead = TimeSpan.FromSeconds(30);
    public static readonly TimeSpan HomeworkQuestionMarkRead = TimeSpan.FromSeconds(10);

    public static TimeSpan GetTtl(string entityType) => entityType switch
    {
        "StudentRead" => StudentRead,
        "InstructorRead" => InstructorRead,
        "EmployeeRead" => EmployeeRead,
        "CourseRead" => CourseRead,
        "CourseStudentRead" => CourseStudentRead,
        "CourseContentRead" => CourseContentRead,
        "AdvertisementRead" => AdvertisementRead,
        "HomeworkRead" => HomeworkRead,
        "HomeworkSubmissionRead" => HomeworkSubmissionRead,
        "HomeworkSolutionRead" => HomeworkSolutionRead,
        "HomeworkQuestionMarkRead" => HomeworkQuestionMarkRead,
        _ => TimeSpan.FromSeconds(30)
    };
}