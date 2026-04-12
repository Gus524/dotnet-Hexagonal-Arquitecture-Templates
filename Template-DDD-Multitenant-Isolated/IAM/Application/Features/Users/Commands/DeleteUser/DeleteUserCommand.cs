using IAM.Application.Features.Users.Common.Ports;
using SharedKernel.Mediator;
using SharedKernel.Ports.In;
using SharedKernel.Wrappers;

namespace IAM.Application.Features.Users.Commands.DeleteUser;

public record DeleteUserCommand(string UserName) : IRequest<Unit>;
public class DeleteUserCommandHandler(
    IUserIdentityRepository userManager
) : ICommandHandler<DeleteUserCommand, Unit>
{
    public async Task<Response<Unit>> Handle(DeleteUserCommand request, CancellationToken cancellationToken)
    {
        var user = await userManager.GetUserAsync(request.UserName, cancellationToken);

        if (user is null)
            return Response<Unit>.NotFound("Usuario no encontrado.");

        var result = await userManager.DeleteUser(user.UserName);
        if (!result)
            return Response<Unit>.NoContent("Error al eliminar el usuario.");

        return Response<Unit>.NoContent("Usuario borrado correctamente.");
    }
}