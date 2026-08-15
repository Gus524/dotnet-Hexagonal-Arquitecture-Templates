using SharedKernel.Abstractions;
using SharedKernel.Enums;
using SharedKernel.Exceptions;

namespace IAM.Domain.Model;

public class Usuario : AggregateRoot<UsuarioId>
{
    public string UserName { get; private set; }
    public string Email { get; private set; }
    public string NombreCompleto { get; private set; }
    public Rol Rol { get; private set; }
    public EstadoUsuario Estado { get; private set; }
    private Usuario(UsuarioId id, string nombreCompleto, string userName, string email, Rol rol) : base(id)
    {
        ValidateUserNameNotEmail(userName, email);
        UserName = userName;
        NombreCompleto = nombreCompleto;
        Email = email;
        Estado = EstadoUsuario.Activo;
        Rol = rol;
    }

    public static Usuario Create(UsuarioId id, string nombreUsuario, string email, string nombreCompleto, Rol rol)
    {
        if (string.IsNullOrWhiteSpace(nombreCompleto)) throw new DomainException("El nombre completo es obligatorio.");
        if (string.IsNullOrWhiteSpace(nombreUsuario)) throw new DomainException("El nombre de usuario es obligatorio.");
        if (string.IsNullOrWhiteSpace(email) || !email.Contains('@')) throw new DomainException("El formato del correo electrónico es inválido.");

        return new Usuario(
            id, nombreCompleto, nombreUsuario, email, rol
        );
    }    

    private static void ValidateUserNameNotEmail(string userName, string email)
    {
        if (userName.Trim().Equals(email.Trim(), StringComparison.OrdinalIgnoreCase))
            throw new DomainException("Por seguridad, el nombre de usuario no puede ser igual al correo electrónico.");
    }

    public void DesactivarUsuario() => Estado = EstadoUsuario.Inactivo;
    public void ActivarUsuario() => Estado = EstadoUsuario.Activo;
}