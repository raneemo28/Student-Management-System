namespace App.domain.entity;

public class Advertisement
{
    public string Advertisement_id { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public DateTime PublishedAt { get; set; } = DateTime.UtcNow;
    public bool IsActive { get; set; } = true;
}
