using IAM.Application.Features.Users.Commands.CreateUser;
using Prestamos.Domain.Model;
using SharedKernel.Enums;

namespace EventsIntegration.Mappers;

public static class PrestatarioToUserCommandMapper
{
    public static CreateUserCommand MapToCommand(PrestatarioRegistrado @event, string originKey = "Integration")
    {
        ArgumentNullException.ThrowIfNull(@event);

        return new CreateUserCommand(
            OriginKey: originKey,
            UserName: @event.UserName,
            Email: @event.Email,
            Password: null,
            Rol: Rol.Cliente
        );
    }
}
