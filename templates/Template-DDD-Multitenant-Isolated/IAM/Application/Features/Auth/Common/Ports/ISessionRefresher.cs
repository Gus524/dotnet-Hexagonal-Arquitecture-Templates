using Core.IAM.Application.Features.Auth.Common.Dtos;

namespace Core.IAM.Application.Features.Auth.Common.Ports;

public interface ISessionRefresher
{
    string HashToken(string token);
    Task<SessionDto> RefreshSession(string userId, CancellationToken ct);
}
