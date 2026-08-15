using IAM.Application.Features.Auth.Common.Dtos;
using IAM.Application.Features.Users.Commands.CreateUser;
using Riok.Mapperly.Abstractions;

namespace IAM.Application.Features.Users.Common.Mappers;

[Mapper(RequiredEnumMappingStrategy = RequiredMappingStrategy.Target)]
public partial class UserMapper
{
    public partial CreateUserDto MapToCreateDto(CreateUserCommand command);
}