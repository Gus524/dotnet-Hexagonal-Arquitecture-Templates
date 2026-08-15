using Core.IAM.Application.Features.Auth.Common.Dtos;
using Core.IAM.Application.Features.Auth.Common.Ports;
using IAM.Domain.Model;
using IAM.Domain.Ports;
using SharedKernel.Mediator;
using SharedKernel.Repository;
using SharedKernel.Wrappers;

namespace IAM.Application.Features.Auth.Commands.IniciarSesion;

public record IniciarSesionCommand(string NombreUsuario, string Password) : IRequest<AuthResponseDto>;

public class IniciarSesionCommandHandler(
    IUserAuthenticator authService,
    IRefreshTokenRepository refreshRepository,
    IUnitOfWork unitOfWork
) : IRequestHandler<IniciarSesionCommand, AuthResponseDto>
{
    public async Task<Response<AuthResponseDto>> Handle(IniciarSesionCommand request, CancellationToken cancellationToken)
    {
        var session = await authService.Authenticate(request.NombreUsuario, request.Password);

        if (session is null)
            return Response.Unauthorized<AuthResponseDto>("Credenciales inválidas.");

        var nuevoToken = RefreshToken.Crear(session.RefreshTokenDto.HashedToken, session.User.Id);

        refreshRepository.Add(nuevoToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        var response = new AuthResponseDto(session.AccessToken, session.RefreshTokenDto.RefreshToken, session.User);

        return Response.Success(response);
    }
}
