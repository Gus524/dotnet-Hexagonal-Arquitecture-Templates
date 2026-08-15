using IAM.Application.Features.Auth.Common.Dtos;
using IAM.Application.Features.Users.Common.Ports;
using SharedKernel.Mediator;
using SharedKernel.Ports.Out.Security;
using SharedKernel.Wrappers;

namespace IAM.Application.Features.Auth.Queries.GetSession;

public record GetSessionQuery : IRequest<AuthUserDto>;

public class GetSessionQueryHandler(
    IReadIdentityRepository userRepository, 
    IUserContext userContext
) : IQueryHandler<GetSessionQuery, AuthUserDto>
{
    public async Task<Response<AuthUserDto>> Handle(GetSessionQuery request, CancellationToken cancellationToken)
    {
        var user = userContext.UserName;
        
        var session = await userRepository.GetUserAsync(user, cancellationToken);
        
        if (session is null)
            return Response.Forbbiden<AuthUserDto>("Error al restarurar sesión.");
        
        return Response.Success(session);
    }
}