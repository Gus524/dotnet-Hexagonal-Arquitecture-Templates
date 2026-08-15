using Core.IAM.Application.Features.Auth.Common.Ports;
using IAM.Domain.Model;
using IAM.Domain.Ports;
using IAM.Domain.Specifications;
using SharedKernel.Mediator;
using SharedKernel.Repository;
using SharedKernel.Wrappers;

namespace IAM.Application.Features.Auth.Commands.CerrarSesion;

public record CerrarSesionCommand(string RefreshToken) : IRequest<bool>;

public class CerrarSesionCommandHandler(
    IRefreshTokenRepository refreshRepository,
    ISessionRefresher sessionRefresher,
    IUnitOfWork unitOfWork
) : IRequestHandler<CerrarSesionCommand, bool>
{
    public async Task<Response<bool>> Handle(CerrarSesionCommand request, CancellationToken cancellationToken)
    {
        var token = await refreshRepository.FindSingleAsync(
            new RefreshTokenByHashedTokenSpecification(sessionRefresher.HashToken(request.RefreshToken)),
            cancellationToken);

        if (token is null)
            return Response.Fail<bool>("El token no existe.");

        token.Revocar();

        await unitOfWork.SaveChangesAsync(cancellationToken);
        return Response.Success(true);
    }
}
