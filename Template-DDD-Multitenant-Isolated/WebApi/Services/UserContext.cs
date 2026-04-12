using System.Security.Claims;
using SharedKernel.Enums;
using SharedKernel.Ports.Out.Security;

namespace WebApi.Services;

public class UserContext(IHttpContextAccessor httpContextAccessor) : IUserContext
{
    private ClaimsPrincipal? User => httpContextAccessor.HttpContext?.User;
    public string UserId => GetRequiredClaim("OriginKey");
    public string UserName => GetRequiredClaim(ClaimTypes.Name);

    private string GetRequiredClaim(string claimType)
    {
        var value = User?.FindFirstValue(claimType);

        if (string.IsNullOrEmpty(value))
        {
            throw new UnauthorizedAccessException("No se encontro el usuario.");
        }

        return value;
    }
    
    public Rol GetCurrentRole()
    {
        var rol = User?.FindFirstValue(ClaimTypes.Role);
        if (rol != null && Enum.TryParse<Rol>(rol, out var role))
        {
            return role;
        }
        throw new UnauthorizedAccessException("No se encontro el rol del usuario.");
    }
}