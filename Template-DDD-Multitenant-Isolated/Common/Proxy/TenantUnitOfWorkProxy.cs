using Microsoft.Extensions.DependencyInjection;
using SharedKernel.Ports.Out.MultiTenancy;
using SharedKernel.Repository;

namespace Common.Proxy;

public class TenantUnitOfWorkProxy(
    ITenantProvider tenantProvider,
    IServiceProvider serviceProvider) : IUnitOfWork
{
    private IUnitOfWork GetUnitOfWork()
    {
        string tenantId = tenantProvider.GetTenantId();
        
        return serviceProvider.GetKeyedService<IUnitOfWork>(tenantId)
            ?? throw new InvalidOperationException($"No IUnitOfWork found for tenant {tenantId}");
    }

    public Task<int> SaveChangesAsync(CancellationToken cancellationToken = default) =>
        GetUnitOfWork().SaveChangesAsync(cancellationToken);
}