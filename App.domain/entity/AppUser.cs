using Microsoft.AspNetCore.Identity;

namespace App.domain.entity;

public class AppUser : IdentityUser
{
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
