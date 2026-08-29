namespace App.Application.DTOs;

public class AdvertisementDto
{
    public string Advertisement_id { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public DateTime PublishedAt { get; set; }
    public bool IsActive { get; set; }
}
