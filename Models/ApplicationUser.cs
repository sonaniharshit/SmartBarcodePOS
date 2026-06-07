using Microsoft.AspNetCore.Identity;

namespace SmartBarcodePOS_Pro.Models;

public class ApplicationUser : IdentityUser
{
    public string FullName { get; set; } = string.Empty;
}
