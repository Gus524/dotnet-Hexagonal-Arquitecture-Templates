using SharedKernel.Enums;

namespace IAM.Application.Features.Auth.Common.Dtos;

// Agregar atributos segun sea necesario para el frontend
public record AuthUserDto(string UserName, string NombreCompleto, Rol Rol, string? Email);