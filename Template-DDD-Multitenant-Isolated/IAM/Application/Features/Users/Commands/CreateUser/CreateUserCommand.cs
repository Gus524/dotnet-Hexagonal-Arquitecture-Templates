using IAM.Application.Features.Users.Common.Mappers;
using IAM.Application.Features.Users.Common.Ports;
using SharedKernel.Enums;
using SharedKernel.Mediator;
using SharedKernel.Wrappers;

namespace IAM.Application.Features.Users.Commands.CreateUser;
public record CreateUserCommand(string OriginKey, string UserName, string Email, string Password, Rol Rol): IRequest<string>;

public class CreateUserCommandHandler(
    IUserIdentityRepository userManager,
    UserMapper mapper
) : ICommandHandler<CreateUserCommand, string>
{
    public async Task<Response<string>> Handle(CreateUserCommand request, CancellationToken cancellationToken)
    {
        var createUser = mapper.MapToCreateDto(request);
        var result = await userManager.CreateUserAsync(createUser, cancellationToken);
        
        if (!result.IsSuccess)
            return Response<string>.BusinessFail("Ocurrió un error al crear el usuario.");
        
        return Response<string>.Success(result.UserName);
    }
}