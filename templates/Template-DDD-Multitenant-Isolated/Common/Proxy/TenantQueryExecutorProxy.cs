using Microsoft.Extensions.DependencyInjection;
using SharedKernel.Ports.Out.MultiTenancy;
using SharedKernel.Repository;

namespace Common.Proxy;

public class TenantQueryExecutorProxy(
    ITenantProvider tenantProvider, 
    IServiceProvider serviceProvider
) : IQueryExecutor
{
    private IQueryExecutor GetExecutor()
    {
        string tenantId = tenantProvider.GetTenantId();
        return serviceProvider.GetKeyedService<IQueryExecutor>(tenantId)
               ?? throw new InvalidOperationException($"No se encontró IQueryExecutor para tenant {tenantId}");
    }

    public Task<IEnumerable<T>> QueryAsync<T>(string query, object? parameters = null,
        CancellationToken cancellationToken = default) =>
        GetExecutor().QueryAsync<T>(query, parameters, cancellationToken);

    public Task<T?> QueryFirstOrDefaultAsync<T>(string sql, object? param = null, CancellationToken ct = default) =>
        GetExecutor().QueryFirstOrDefaultAsync<T>(sql, param, ct);
}