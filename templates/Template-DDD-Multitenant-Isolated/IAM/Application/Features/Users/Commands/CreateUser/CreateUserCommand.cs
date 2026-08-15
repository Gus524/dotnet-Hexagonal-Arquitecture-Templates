using IAM.Application.Features.Users.Common.Ports;
using IAM.Domain.Model;
using SharedKernel.Enums;
using SharedKernel.Mediator;
using SharedKernel.Wrappers;
using UserMapper = IAM.Application.Features.Users.Common.Mappers.UserMapper;

namespace IAM.Application.Features.Users.Commands.CreateUser;
public record CreateUserCommand(string OriginKey, string UserName, string Email, string? Password = null, Rol Rol = Rol.Cliente): IRequest<string>;

public class CreateUserCommandHandler(
    IUserIdentityRepository userManager,
    UserMapper mapper
) : ICommandHandler<CreateUserCommand, string>
{
    public async Task<Response<string>> Handle(CreateUserCommand request, CancellationToken cancellationToken)
    {
        var effectivePassword = string.IsNullOrWhiteSpace(request.Password)
            ? $"Temp@{Guid.NewGuid().ToString("N")[..8]}!"
            : request.Password;

        var commandWithPassword = request with { Password = effectivePassword };
        var createUser = mapper.MapToCreateDto(commandWithPassword);
        var result = await userManager.CreateUserAsync(createUser, cancellationToken);
        
        if (!result.IsSuccess)
            return Response.BusinessFail<string>("Ocurrió un error al crear el usuario.");
        
        return Response.Success(result.UserName);
    }
}