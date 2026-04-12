using IAM.Application.Features.Auth.Common.Mappers;
using IAM.Application.Features.Auth.Common.Ports;
using IAM.Application.Features.Auth.Common.Responses;
using IAM.Domain.Entities;

namespace IAM.Application.Features.Auth.Common.Services;

public class LoginService(IAuthPort authPort, AuthMapper mapper) : ILoginService
{
    public async Task<AuthResponse?> ProcesarAutenticacion(string userName, string password, Usuario userInfo)
    {
        var result = await authPort.AutenticarUsuario(userName, password);

        if (!result.IsSuccess)
            return null;

        var user = mapper.MapToAuthDto(userInfo);
        var response = new AuthResponse(result.Token!, user);
        
        return response;
    }
}