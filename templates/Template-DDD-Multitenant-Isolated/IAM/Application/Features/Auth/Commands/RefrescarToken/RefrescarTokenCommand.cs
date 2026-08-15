using Core.IAM.Application.Features.Auth.Common.Ports;
using IAM.Domain.Model;
using IAM.Domain.Ports;
using IAM.Domain.Specifications;
using SharedKernel.Mediator;
using SharedKernel.Repository;
using SharedKernel.Wrappers;

namespace IAM.Application.Features.Auth.Commands.RefrescarToken;

public record TokensResponse(string AccessToken, string RefreshToken);

public record RefrescarTokenCommand(string RefreshToken) : IRequest<TokensResponse>;

public class RefrescarTokenCommandHandler(
    IRefreshTokenRepository refreshRepository,
    ISessionRefresher sessionRefresher,
    IUnitOfWork unitOfWork
) : IRequestHandler<RefrescarTokenCommand, TokensResponse>
{
    public async Task<Response<TokensResponse>> Handle(RefrescarTokenCommand request, CancellationToken cancellationToken)
    {
        var token = await refreshRepository.FindSingleAsync(
            new RefreshTokenByHashedTokenSpecification(sessionRefresher.HashToken(request.RefreshToken)),
            cancellationToken);

        if (token is null)
            return Response.Fail<TokensResponse>("Token inválido.");

        if (token.RevokedAt.HasValue)
        {
            var activeTokens = await refreshRepository.FindAsync(
                new TokenByUserSpecification(token.UsuarioId),
                cancellationToken);

            foreach (var t in activeTokens)
            {
                t.Revocar();
            }

            await unitOfWork.SaveChangesAsync(cancellationToken);

            return Response.Fail<TokensResponse>("Sesión cerrada por seguridad. Por favor, inicia sesión de nuevo.");
        }

        if (token.IsExpired)
            return Response.Fail<TokensResponse>("Sesión expirada. Por favor, inicia sesión de nuevo.");

        token.Revocar();

        var session = await sessionRefresher.RefreshSession(token.UsuarioId, cancellationToken);
        var nuevoToken = RefreshToken.Crear(session.RefreshTokenDto.HashedToken, token.UsuarioId);

        refreshRepository.Add(nuevoToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        var response = new TokensResponse(session.AccessToken, session.RefreshTokenDto.RefreshToken);

        return Response.Success(response);
    }
}
