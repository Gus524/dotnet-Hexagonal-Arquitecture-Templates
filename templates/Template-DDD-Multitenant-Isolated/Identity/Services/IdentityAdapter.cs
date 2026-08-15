using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Core.IAM.Application.Features.Auth.Common.Dtos;
using Core.IAM.Application.Features.Auth.Common.Ports;
using IAM.Application.Features.Auth.Common.Dtos;
using Identity.Data;
using Identity.Settings;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;
using SharedKernel.Enums;

namespace Identity.Services;

public class IdentityAdapter(
    IOptions<JwtSettings> jwtSettings,
    UserManager<ApplicationUser> userManager,
    ILogger<IdentityAdapter> logger
) : IUserAuthenticator, ISessionRefresher, IUserProfileQuery, IAuthPort
{
    private readonly JwtSettings _jwtSettings = jwtSettings.Value;

    public async Task<SessionDto?> Authenticate(string userName, string password)
    {
        var user = await userManager.FindByNameAsync(userName);
        if (user is null)
        {
            logger.LogWarning("Validación fallida: no se encontró el usuario {UserName}", userName);
            return null;
        }

        var passwordValid = await userManager.CheckPasswordAsync(user, password);
        if (!passwordValid)
        {
            logger.LogWarning("Validación fallida: contraseña no válida para {UserName}", userName);
            return null;
        }

        return await CrearSesionAsync(user);
    }

    public async Task<(bool IsSuccess, string? Token)> AutenticarUsuario(string nombreUsuario, string password)
    {
        var session = await Authenticate(nombreUsuario, password);
        return session is null ? (false, null) : (true, session.AccessToken);
    }

    public string HashToken(string token)
    {
        var bytes = Encoding.UTF8.GetBytes(token);
        var hash = SHA256.HashData(bytes);
        return Convert.ToHexString(hash);
    }

    public async Task<SessionDto> RefreshSession(string userId, CancellationToken ct)
    {
        var user = await userManager.FindByIdAsync(userId) 
            ?? await userManager.FindByNameAsync(userId);

        if (user is null)
        {
            logger.LogWarning("No se encontró el usuario para refrescar la sesión: {UserId}", userId);
            throw new InvalidOperationException($"No se encontró el usuario con ID {userId}.");
        }

        return await CrearSesionAsync(user);
    }

    public async Task<AuthUserDto?> GetUserSession(string userId)
    {
        var user = await userManager.FindByIdAsync(userId) 
            ?? await userManager.FindByNameAsync(userId);

        if (user is null) return null;

        var roles = await userManager.GetRolesAsync(user);
        var roleString = roles.FirstOrDefault();

        Enum.TryParse<Rol>(roleString, true, out var domainRol);

        return new AuthUserDto(user.Id, user.UserName ?? string.Empty, user.NombreCompleto, domainRol, user.Email);
    }

    private async Task<SessionDto> CrearSesionAsync(ApplicationUser user)
    {
        var claims = await BuildClaims(user);
        var accessToken = GenerarJwtToken(claims);

        var rawRefreshToken = Convert.ToBase64String(RandomNumberGenerator.GetBytes(64));
        var hashedToken = HashToken(rawRefreshToken);
        var refreshTokenDto = new RefreshTokenDto(rawRefreshToken, hashedToken);

        var authUser = await GetUserSession(user.Id);

        return new SessionDto(accessToken, refreshTokenDto, authUser!);
    }

    private async Task<List<Claim>> BuildClaims(ApplicationUser user)
    {
        var roles = await userManager.GetRolesAsync(user);
        var claims = new List<Claim>
        {
            new(JwtRegisteredClaimNames.Sub, user.Id),
            new(ClaimTypes.NameIdentifier, user.Id),
            new(ClaimTypes.Name, user.UserName ?? string.Empty),
            new(ClaimTypes.Email, user.Email ?? string.Empty),
            new("OriginKey", user.OriginKey),
            new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        };

        foreach (var rol in roles)
        {
            claims.Add(new Claim(ClaimTypes.Role, rol));
        }

        return claims;
    }

    private string GenerarJwtToken(IEnumerable<Claim> claims)
    {
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSettings.JWT_Secret));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Issuer = _jwtSettings.JWT_ISSUER_TOKEN,
            Audience = _jwtSettings.JWT_AUDIENCE_TOKEN,
            Subject = new ClaimsIdentity(claims),
            Expires = DateTime.UtcNow.AddMinutes(_jwtSettings.ExpiryInMinutes),
            SigningCredentials = creds
        };

        var handler = new JsonWebTokenHandler();
        return handler.CreateToken(tokenDescriptor);
    }
}
