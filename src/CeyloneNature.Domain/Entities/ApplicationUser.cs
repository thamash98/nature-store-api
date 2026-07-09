using Microsoft.AspNetCore.Identity;

namespace CeyloneNature.Domain.Entities;

public class ApplicationUser : IdentityUser<int>
{
    public string Name { get; set; } = "";
    public int LoyaltyPoints { get; set; } = 0;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
