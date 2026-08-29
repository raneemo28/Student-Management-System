namespace App.Application.DTOs;

public class CreateHomeworkSolutionDto
{
    public string Homework_id { get; set; } = string.Empty;
    public string SolutionFileName { get; set; } = string.Empty;
    public byte[] SolutionFileBytes { get; set; } = Array.Empty<byte>();
    public string? Notes { get; set; }
}
