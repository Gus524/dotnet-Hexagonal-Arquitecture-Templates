using SharedKernel.Enums;

namespace IAM.Application.Features.Auth.Common.Dtos;

public record CreateUserDto(string OriginKey, string UserName, string Password, Rol Rol, string? Email);