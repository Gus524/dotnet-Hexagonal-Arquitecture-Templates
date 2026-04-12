using Microsoft.Extensions.Configuration;
using SharedKernel.Ports.Out.MultiTenancy;

namespace Identity.Services;

public class ConnectionResolver(ITenantProvider tenantProvider, IConfiguration configuration) : IConnectionResolver
{
    public string GetConnectionString()
    {
        var tenantId = tenantProvider.GetTenantId();
        
        return configuration.GetConnectionString(tenantId)
            ?? throw new InvalidOperationException($"No connection string found for tenant '{tenantId}'.");
    }
}
