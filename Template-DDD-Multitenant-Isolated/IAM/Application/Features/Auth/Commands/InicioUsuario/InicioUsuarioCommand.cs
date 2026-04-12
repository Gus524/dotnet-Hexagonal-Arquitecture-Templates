using IAM.Application.Features.Auth.Common.Mappers;
using IAM.Application.Features.Auth.Common.Ports;
using IAM.Application.Features.Auth.Common.Responses;
using IAM.Application.Features.Users.Common.Ports;
using SharedKernel.Mediator;
using SharedKernel.Ports.In;
using SharedKernel.Wrappers;

namespace IAM.Application.Features.Auth.Commands.InicioUsuario;

public record InicioUsuarioCommand(string UserName, string Password) : IRequest<AuthResponse>;

public class InicioUsuarioCommandHandler(
    IUserIdentityRepository userIdentityRepository,
    IAuthPort authPort,
    AuthMapper mapper
) : ICommandHandler<InicioUsuarioCommand, AuthResponse>
{
    public async Task<Response<AuthResponse>> Handle(InicioUsuarioCommand request, CancellationToken cancellationToken)
    {
        var result = await authPort.AutenticarUsuario(request.UserName, request.Password);

        if (!result.IsSuccess)
            return Response<AuthResponse>.Unauthorized();;

        var user = await userIdentityRepository.GetUserAsync(request.UserName, cancellationToken);
        if (user is null)
            return Response<AuthResponse>.Unauthorized();
        
        var sesion = new AuthResponse(result.Token!, user);
        
        return Response<AuthResponse>.Success(sesion);
    }
}