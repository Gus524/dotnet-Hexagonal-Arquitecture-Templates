using System.IdentityModel.Tokens.Jwt;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;
using System.Text;
using IAM.Application.Features.Auth.Common.Ports;
using Identity.Data;
using Identity.Settings;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Identity.Services;

public class AuthAdapter(
    IOptions<JwtSettings> jwtSettings,
    UserManager<ApplicationUser> userManager,
    ILogger<AuthAdapter> logger
) : IAuthPort
{
    private readonly JwtSettings _jwtSettings = jwtSettings.Value;
    public async Task<(bool IsSuccess, string? Token)> AutenticarUsuario(string nombreUsuario, string password)
    {
        var user = await userManager.FindByNameAsync(nombreUsuario);
        if (user == null)
        {
            logger.LogWarning("Validacion fallida, no se encontro usuario: {nombreUsuario}", nombreUsuario);
            return (false, null);
        }

        var passwordValid = await userManager.CheckPasswordAsync(user, password);
        if (passwordValid)
            return (true, await GenerarToken(user));
        
        logger.LogWarning("Validacion fallida, contraseña no valida para: {nombreUsuario}", nombreUsuario);
        return (false, null);
    }

    private async Task<string> GenerarToken(ApplicationUser user)
    {
        var claims = await GetClaims(user);
        
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSettings.JWT_Secret));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: _jwtSettings.JWT_ISSUER_TOKEN,
            audience: _jwtSettings.JWT_AUDIENCE_TOKEN,
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(_jwtSettings.ExpiryInMinutes),
            signingCredentials: creds
        );
        
        return new JwtSecurityTokenHandler().WriteToken(token);
    }
    private async Task<List<Claim>> GetClaims(ApplicationUser user)
    {
        var roles = await userManager.GetRolesAsync(user);
        var claims = new List<Claim>
        {
            new(JwtRegisteredClaimNames.Sub, user.Id),
            new(ClaimTypes.NameIdentifier, user.Id),
            new(ClaimTypes.Name, user.UserName!),
            new(ClaimTypes.Email, user.Email!),
            new("OriginKey", user.OriginKey),
            new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        };

        foreach (var rol in roles)
        {
            claims.Add(new Claim(ClaimTypes.Role, rol));
        }

        return claims;
    }
}