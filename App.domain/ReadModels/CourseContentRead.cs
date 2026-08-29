namespace App.domain.ReadModels;

public class CourseContentRead
{
    public string Content_id { get; set; } = string.Empty;
    public string Course_id { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string ContentType { get; set; } = string.Empty;
    public string FileName { get; set; } = string.Empty;
    public string FileUrl { get; set; } = string.Empty;
    public long FileSize { get; set; }
    public DateTime CreatedAt { get; set; }
}
