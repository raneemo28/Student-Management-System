namespace App.Application.DTOs;

public class InstructorDto
{
    public string Instructor_id { get; set; } = string.Empty;
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string? User_id { get; set; }
}
