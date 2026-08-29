namespace App.Application.DTOs;

public class UpdateCourseContentDto
{
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string ContentType { get; set; } = string.Empty;
    public string? FileName { get; set; }
    public long? FileSize { get; set; }
    public byte[]? FileBytes { get; set; }
}
