using IAM.Application.Features.Auth.Common.Mappers;
using IAM.Application.Features.Users.Common.Mappers;
using SharedKernel.Behaviors;
using SharedKernel.Ports.In;
using System.Reflection;
using SharedKernel.Mediator;
using WebApi.Mediator;

namespace WebApi.Extensions.DependencyInjection;

/// <summary>
/// Orquesta el registro dinámico del motor CQRS y la tubería de comportamientos (Pipeline Behaviors).
/// </summary>
/// <remarks>
/// Utiliza escaneo de ensamblados para descubrir y registrar automáticamente 
/// todos los Casos de Uso (<c>IRequestHandler</c>) y Validadores, evitando el registro manual tedioso.
/// Además, ensambla el mediador interno y acopla el <c>ValidationBehavior</c> como paso previo obligatorio 
/// en la ejecución de cualquier comando o consulta.
/// </remarks>
public static class ApplicationExtensions
{
    public static void AddApplicationLayer(this IServiceCollection services, params Assembly[] assemblies)
    {
        services.Scan(s => s.FromAssemblies(assemblies)
            .AddClasses(c => c.AssignableTo(typeof(IRequestHandler<,>)))
            .AsImplementedInterfaces()
            .WithTransientLifetime());

        services.Scan(s => s.FromAssemblies(assemblies)
            .AddClasses(c => c.AssignableTo(typeof(IValidator<>)))
            .AsImplementedInterfaces()
            .WithTransientLifetime());

        services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));
        services.AddTransient<IMediator, InternalMediator>();

        services.AddScoped<UserMapper>();
        services.AddScoped<AuthMapper>();
    }
}