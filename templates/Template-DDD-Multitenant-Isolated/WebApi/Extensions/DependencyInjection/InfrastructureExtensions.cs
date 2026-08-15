using Common.Extensions;
using Common.Proxy;
using FileStorage;
using Identity;
using Prestamos.Domain.Ports;
using ProjectExample;
using Shared;
using SharedKernel.Repository;

namespace WebApi.Extensions.DependencyInjection;

public static class InfrastructureExtensions
{
    public static void AddInfrastructureLayer(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddScoped(typeof(IRepository<,>), typeof(TenantRepositoryProxy<,>));
        services.AddScoped<IUnitOfWork, TenantUnitOfWorkProxy>();
        services.AddScoped<IQueryExecutor, TenantQueryExecutorProxy>();
        
        services.AddCoreRepositoryProxies(typeof(IPrestatarioRepository).Assembly);
        
        // services.AddEventStoreInfrastructure();
        services.AddIdentityInfraestructure(configuration);
        services.AddSharedInfraestructure();
        services.AddFileStorageInfraestructure();
        
        services.AddProjectExamplePersistence(configuration);
    }
}