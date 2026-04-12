namespace SharedKernel.Ports.Out.MultiTenancy;

/// <summary>
/// Aísla la resolución del contexto del inquilino (Tenant) de su origen físico, 
/// permitiendo a la lógica de negocio operar en un entorno multitenancy sin acoplarse 
/// a los protocolos de red (ej. HTTP Headers, JWT, Subdominios).
/// </summary>
public interface ITenantProvider
{
    string GetTenantId();
    void SetTenantId(string tenantId);
}

/// <summary>
/// Centraliza los identificadores estáticos de los inquilinos del sistema para evitar cadenas mágicas (magic strings)
/// a lo largo de la aplicación, facilitando la navegación y el mantenimiento.
/// </summary>
public static class TenantConstants
{
    public const string ProjectExample = "ProjectExample";
}