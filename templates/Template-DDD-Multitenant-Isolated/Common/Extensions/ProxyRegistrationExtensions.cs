using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using SharedKernel.Ports.Out.MultiTenancy;
using SharedKernel.Repository;

namespace Common.Extensions;

public static class ProxyRegistrationExtensions
{
    /// <summary>
    /// Escanea el ensamblado de Dominio/Core y registra automáticamente la resolución Proxy
    /// para TODAS las interfaces de repositorio del dominio (ej. IPrestatarioRepository)
    /// resolviendo dinámicamente el KeyedService asociado al tenant activo.
    /// </summary>
    public static void AddCoreRepositoryProxies(this IServiceCollection services, Assembly coreAssembly)
    {
        services.Scan(scan => scan
            .FromAssemblies(coreAssembly)
            .AddClasses(classes => classes.Where(_ => false)) // No registramos clases concretas aquí
            .UsingRegistrationStrategy(Scrutor.RegistrationStrategy.Skip)
            .AsSelf());

        // Buscamos todas las interfaces del Core que extienden IRepository<,> (puertos DDD)
        var repositoryInterfaces = coreAssembly.GetTypes()
            .Where(t => t.IsInterface && !t.IsGenericTypeDefinition)
            .Where(t => t.GetInterfaces().Any(i => 
                i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IRepository<,>)))
            .ToList();

        foreach (var repoInterface in repositoryInterfaces)
        {
            services.AddScoped(repoInterface, sp =>
            {
                var tenantProvider = sp.GetRequiredService<ITenantProvider>();
                var tenantId = tenantProvider.GetTenantId();

                return sp.GetKeyedService(repoInterface, tenantId) ??
                       throw new InvalidOperationException(
                           $"Error Multitenant: No se encontró una implementación de '{repoInterface.Name}' registrada para el tenant '{tenantId}'.");
            });
        }
    }
}