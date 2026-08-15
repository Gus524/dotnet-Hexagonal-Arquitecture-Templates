namespace Core.IAM.Application.Features.Auth.Common.Ports;

public interface IAuthPort
{
    Task<(bool IsSuccess, string? Token)> AutenticarUsuario(string nombreUsuario, string password);
}
