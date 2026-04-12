using SharedKernel.Enums;

namespace SharedKernel.Ports.Out.Security;

/// <summary>
/// Provee una vista purgada y segura del usuario autenticado que está ejecutando la acción actual.
/// </summary>
/// <remarks>
/// Desvincula a las capas internas de la infraestructura de seguridad web (como <c>HttpContext</c> o <c>ClaimsPrincipal</c>), 
/// permitiendo que la capa de dominio asigne auditorías y permisos de forma aislada y testeable 
/// (ej. en pruebas unitarias o procesos en segundo plano).
/// </remarks>
public interface IUserContext
{
    string UserId { get; }
    string UserName { get; }
    Rol GetCurrentRole();
}