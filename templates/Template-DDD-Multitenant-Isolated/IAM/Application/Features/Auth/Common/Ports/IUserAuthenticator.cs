using Core.IAM.Application.Features.Auth.Common.Dtos;

namespace Core.IAM.Application.Features.Auth.Common.Ports;

public interface IUserAuthenticator
{
    Task<SessionDto?> Authenticate(string userName, string password);
}
