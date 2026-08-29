namespace App.Application.DTOs;

public class UpdateAdvertisementDto
{
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public bool IsActive { get; set; }
}
