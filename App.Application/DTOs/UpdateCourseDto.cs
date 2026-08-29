namespace App.Application.DTOs;

public class UpdateCourseDto
{
    public string Course_name { get; set; } = string.Empty;
    public string? Instructor_id { get; set; }
}
