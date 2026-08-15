using IAM.Application.Features.Auth.Common.Dtos;
using IAM.Application.Features.Users.Common.Ports;
using Identity.Data;
using Microsoft.AspNetCore.Identity;
using SharedKernel.Enums;

namespace Identity.Services;

public class UserIdentityRepository(
    UserManager<ApplicationUser> userManager
) : IUserIdentityRepository
{
    public async Task<(bool IsSuccess, string UserName)> CreateUserAsync(CreateUserDto dto, CancellationToken cancellationToken = default)
    {
        var user = new ApplicationUser
        {
            OriginKey = dto.OriginKey,
            UserName = dto.UserName,
            Email = dto.Email
        };

        var result = await userManager.CreateAsync(user, dto.Password);

        if (!result.Succeeded)
        {
            return (false, string.Empty);
        }

        await userManager.AddToRoleAsync(user, dto.Rol.ToString());

        return (true, user.UserName);
    }

    public async Task<bool> UpdateUser(string userName, string newUserName)
    {
        var user = await userManager.FindByNameAsync(userName);
        
        if (user is null) return false;

        user.UserName = newUserName;
        
        var result = await userManager.UpdateAsync(user);

        return result.Succeeded;
    }

    public async Task<bool> ChangePassword(string userName, string oldPassword, string newPassword)
    {
        var user = await userManager.FindByNameAsync(userName);

        if (user is null) return false;

        var result = await userManager.ChangePasswordAsync(user, oldPassword, newPassword);

        return result.Succeeded;
    }

    public async Task<bool> DeleteUser(string userName)
    {
        var user = await userManager.FindByNameAsync(userName);

        if (user is null) 
        {
            return false; 
        }

        var result = await userManager.DeleteAsync(user);

        return result.Succeeded;
    }

    public async Task<AuthUserDto?> GetUserAsync(string userName, CancellationToken cancellationToken = default)
    {
        var identity = await userManager.FindByNameAsync(userName);
        if (identity is null) return null;
        
        var roles = await userManager.GetRolesAsync(identity);
        
        var roleString = roles.FirstOrDefault();
        
        if (!Enum.TryParse<Rol>(roleString, true, out var domainRol))
        {
            throw new UnauthorizedAccessException("El usuario no tiene un rol valido.");
        }

        return new AuthUserDto(identity.Id, identity.UserName!, identity.NombreCompleto, domainRol, identity.Email);
    }
}