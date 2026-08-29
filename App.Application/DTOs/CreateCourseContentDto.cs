namespace App.Application.DTOs;

public class CreateCourseContentDto
{
    public string Course_id { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string ContentType { get; set; } = string.Empty;
    public string FileName { get; set; } = string.Empty;
    public long FileSize { get; set; }
    public byte[] FileBytes { get; set; } = Array.Empty<byte>();
}
