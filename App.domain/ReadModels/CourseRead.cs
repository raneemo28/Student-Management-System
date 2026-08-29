namespace App.domain.ReadModels;

public class CourseRead
{
    public string Course_id { get; set; } = string.Empty;
    public string Course_name { get; set; } = string.Empty;
    public string? Instructor_id { get; set; }
}
