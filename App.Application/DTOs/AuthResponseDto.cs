namespace App.Application.DTOs;

public class AuthResponseDto
{
    public string Token { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string UserId { get; set; } = string.Empty;
    public string StudentId { get; set; } = string.Empty;
    public string InstructorId { get; set; } = string.Empty;
    public string EmployeeId { get; set; } = string.Empty;
}
