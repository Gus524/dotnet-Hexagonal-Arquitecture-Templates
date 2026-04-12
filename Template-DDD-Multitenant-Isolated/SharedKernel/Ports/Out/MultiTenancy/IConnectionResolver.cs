namespace SharedKernel.Ports.Out.MultiTenancy;

/// <summary>
/// Aísla la resolución del contexto del inquilino (Tenant) de su origen físico, 
/// permitiendo a la lógica de negocio operar en un entorno multitenancy sin acoplarse 
/// a los protocolos de red (ej. HTTP Headers, JWT, Subdominios).
/// </summary>
public interface IConnectionResolver
{
    string GetConnectionString();
}