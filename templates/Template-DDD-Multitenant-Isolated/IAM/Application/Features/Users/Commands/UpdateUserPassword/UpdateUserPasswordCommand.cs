using IAM.Application.Features.Users.Common.Ports;
using SharedKernel.Mediator;
using SharedKernel.Wrappers;

namespace IAM.Application.Features.Users.Commands.UpdateUserPassword;

public record UpdateUserPasswordCommand(string UserName, string Password, string NewPassword) : IRequest<string>;

public class UpdateUserPasswordCommandHandler(
    IUserIdentityRepository userManager
) : ICommandHandler<UpdateUserPasswordCommand, string>
{
    public async Task<Response<string>> Handle(UpdateUserPasswordCommand request, CancellationToken cancellationToken)
    {
        var success = await userManager.ChangePassword(request.UserName, request.Password, request.NewPassword);

        if (!success)
            return Response.Fail<string>(
                "Error al actualizar la contraseña, verifique que cuente con las caracteristicas necesarias.");
        
        return Response.Success(request.UserName, "Contraseña actualizada correctamente.");
    }
}