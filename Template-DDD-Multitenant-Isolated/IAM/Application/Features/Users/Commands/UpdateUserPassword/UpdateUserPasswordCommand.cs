using IAM.Application.Features.Users.Common.Ports;
using IAM.Domain.Entities;
using SharedKernel.Mediator;
using SharedKernel.Ports.In;
using SharedKernel.Ports.Out.Repository;
using SharedKernel.Repository;
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
            return Response<string>.Fail("Error al actualizar la contraseña, verifique que cuente con las caracteristicas necesarias.");
        
        return Response<string>.Success(request.UserName, "Contraseña actualizada correctamente.");
    }
}