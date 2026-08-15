using IAM.Application.Features.Auth.Common.Dtos;
using IAM.Domain.Model;
using Riok.Mapperly.Abstractions;

namespace IAM.Application.Features.Auth.Common.Mappers;

[Mapper(RequiredMappingStrategy = RequiredMappingStrategy.Target)]
public partial class AuthMapper
{
    public partial AuthUserDto MapToAuthDto(Usuario usuario);
}