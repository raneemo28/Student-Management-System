namespace App.Application.DTOs;

public class CreateHomeworkSubmissionDto
{
    public string Homework_id { get; set; } = string.Empty;
    public string Student_id { get; set; } = string.Empty;
    public string SolutionFileName { get; set; } = string.Empty;
    public byte[] SolutionFileBytes { get; set; } = Array.Empty<byte>();
}
