namespace App.Application.DTOs;

public class CreateHomeworkDto
{
    public string Course_id { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public DateTime DueDate { get; set; }
    public int TotalMarks { get; set; }
    public string QuestionFileName { get; set; } = string.Empty;
    public byte[] QuestionFileBytes { get; set; } = Array.Empty<byte>();
}
