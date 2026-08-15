using Core.IAM.Application.Features.Auth.Common.Dtos;
using IAM.Application.Features.Auth.Common.Dtos;

namespace Core.IAM.Application.Features.Auth.Common.Ports;

public interface IUserProfileQuery
{
    Task<AuthUserDto?> GetUserSession(string userId);
}
