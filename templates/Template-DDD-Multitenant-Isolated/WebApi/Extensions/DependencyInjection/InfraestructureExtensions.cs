using Common.Proxy;
using FileStorage;
using Identity;
using Shared;
using SharedKernel.Repository;

namespace WebApi.Extensions.DependencyInjection;

public static class InfraestructureExtensions
{
    public static void AddInfraestructureLayer(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddScoped(typeof(IRepository<,>), typeof(TenantRepositoryProxy<,>));
        services.AddScoped<IUnitOfWork, TenantUnitOfWorkProxy>();
        
        services.AddSharedInfraestructure();
        services.AddIdentityInfraestructure(configuration);
        services.AddFileStorageInfraestructure();
    }
}