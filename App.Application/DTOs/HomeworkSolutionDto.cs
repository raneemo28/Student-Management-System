namespace App.Application.DTOs;

public class HomeworkSolutionDto
{
    public string Solution_id { get; set; } = string.Empty;
    public string Homework_id { get; set; } = string.Empty;
    public string SolutionFileName { get; set; } = string.Empty;
    public string SolutionFileUrl { get; set; } = string.Empty;
    public DateTime UploadedAt { get; set; }
    public string? Notes { get; set; }
}
