using Microsoft.AspNetCore.Identity;

namespace Identity.Data;

public class ApplicationUser : IdentityUser
{
    public string OriginKey { get; set; } = string.Empty;
    public string NombreCompleto { get; set; } = string.Empty;
}