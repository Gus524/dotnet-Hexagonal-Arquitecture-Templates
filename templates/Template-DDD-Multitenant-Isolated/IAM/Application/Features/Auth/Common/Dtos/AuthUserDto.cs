using SharedKernel.Enums;

namespace IAM.Application.Features.Auth.Common.Dtos;

public record AuthUserDto(string Id, string UserName, string NombreCompleto, Rol Rol, string? Email);