namespace App.Application.DTOs;

public class StudentDto
{
    public string Student_id { get; set; } = string.Empty;
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string? User_id { get; set; }
}
