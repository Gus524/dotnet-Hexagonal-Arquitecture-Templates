using SharedKernel.Enums;

namespace Identity.Data;

public class IdentityRoles
{
    public static readonly string Administrador = nameof(Rol.Administrador);
    public static readonly string Usuario = nameof(Rol.Usuario);

    public static List<string> AllRoles => 
        Enum.GetNames(typeof(Rol)).ToList();
}