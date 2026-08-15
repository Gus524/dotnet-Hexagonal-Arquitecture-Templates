using System.Reflection;
using Common.Mappers;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using SharedKernel.Repository;

namespace Common.Extensions;

/// <summary>
/// Automatiza el descubrimiento y registro de implementaciones de repositorios dentro del contenedor 
/// de inyección de dependencias, segmentándolos obligatoriamente bajo la llave del inquilino especificado.
/// </summary>
/// <remarks>
/// Oculta la engorrosa lógica de reflexión (Reflection) y evita el registro manual en el arranque.
/// Vincula dinámicamente las interfaces genéricas a las instancias concretas, posibilitando el escalado 
/// fluido y sin errores humanos al incorporar nuevas entidades al sistema multitenancy.
/// </remarks>
public static class KeyedServiceExtensions
{
    public static void RegisterTenantInfrastructure<TContext>(
        this IServiceCollection services, 
        Assembly assembly, 
        string tenantKey) where TContext : DbContext, IUnitOfWork
    {
        services.AddKeyedScoped<IUnitOfWork>(tenantKey, (sp, key) => 
        {
            var context = sp.GetService<TContext>();
            if (context == null)
            {
                throw new InvalidOperationException(
                    $"Error de Configuración: El DbContext '{typeof(TContext).Name}' " +
                    $"no ha sido registrado. Asegúrate de llamar a AddDbContext<{typeof(TContext).Name}> " +
                    $"antes de RegisterTenantInfraestructure.");
            }
            return context;
        });
        
        var types = assembly.GetTypes()
            .Where(t => t is { IsClass: true, IsAbstract: false })
            .ToList();

        foreach (var type in types)
        {
            var interfaces = type.GetInterfaces();

            foreach (var iface in interfaces)
            {
                bool isRepo = iface.IsGenericType && iface.GetGenericTypeDefinition() == typeof(IRepository<,>) ||
                              iface.GetInterfaces().Any(i =>
                                  i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IRepository<,>));
                
                if (isRepo)
                {
                    services.AddKeyedScoped(iface, tenantKey, type);
                }

                bool isMapper = iface.IsGenericType && 
                                (iface.GetGenericTypeDefinition() == typeof(IMapper<,>) || 
                                 iface.GetGenericTypeDefinition() == typeof(IDtoMapper<,>));

                if (isMapper)
                {
                    services.AddKeyedScoped(iface, tenantKey, type);
                }
            }
        }
    }
}
