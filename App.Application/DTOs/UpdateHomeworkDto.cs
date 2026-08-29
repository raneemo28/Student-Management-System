namespace App.Application.DTOs;

public class UpdateHomeworkDto
{
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public DateTime DueDate { get; set; }
    public int TotalMarks { get; set; }
    public string? QuestionFileName { get; set; }
    public byte[]? QuestionFileBytes { get; set; }
}
