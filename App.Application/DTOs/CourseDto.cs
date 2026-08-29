namespace App.Application.DTOs;

public class CourseDto
{
    public string Course_id { get; set; } = string.Empty;
    public string Course_name { get; set; } = string.Empty;
    public string? Instructor_id { get; set; }
    public double WorkWeight { get; set; }
    public double FirstExamWeight { get; set; }
    public double SecondExamWeight { get; set; }
    public double FinalExamWeight { get; set; }
}
