using SharedKernel.Ports.Out;
using SharedKernel.Ports.Out.Repository;

namespace WebApi.Extensions.Bootstrap;

/// <summary>
/// Coordina la fase de arranque (Bootstrap) para la inicialización y migración automática de repositorios de datos.
/// </summary>
/// <remarks>
/// En un entorno multitenancy con múltiples bases de datos, este módulo evita la orquestación manual de cada contexto.
/// Resuelve todos los componentes que implementan <c>IDbContextInitializer</c> en el contenedor y los ejecuta,
/// garantizando que la infraestructura física esté construida y alineada antes de procesar tráfico web.
/// </remarks>
public static class InitializeExtensions
{
    public static async Task InitializeDatabasesAsync(this WebApplication app)
    {
        using var scope = app.Services.CreateScope();

        var initializers = scope.ServiceProvider.GetServices<IDbContextInitializer>();
        foreach (var initializer in initializers)
        {
            await initializer.InitializeAsync();
        }
    }
}