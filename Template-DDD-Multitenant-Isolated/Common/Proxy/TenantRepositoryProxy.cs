using Microsoft.Extensions.DependencyInjection;
using SharedKernel.Abstractions;
using SharedKernel.Ports.Out.MultiTenancy;
using SharedKernel.Repository;
using SharedKernel.Specification;

namespace Common.Proxy;

public class TenantRepositoryProxy<TAggregate, TId>(
    ITenantProvider tenantProvider,
    IServiceProvider serviceProvider) : IRepository<TAggregate, TId> 
    where TAggregate : class, IAggregateRoot
{
    private IRepository<TAggregate, TId> GetRepository()
    {
        string tenantId;
        
        try
        {
            tenantId = tenantProvider.GetTenantId();
        }
        catch (InvalidOperationException ex) when (ex.Message.Contains("HttpContext"))
        {
            throw new InvalidOperationException(
                $"No se pudo determinar el TenantId en el contexto actual. " +
                $"Si está en un background job, use SetTenantId() en ITenantProvider. " +
                $"Si está en tests, proporcione un mock de ITenantProvider.",
                ex
            );
        }

        return serviceProvider.GetKeyedService<IRepository<TAggregate, TId>>(tenantId)
               ?? throw new InvalidOperationException(
                   $"No se encontró repositorio para el tenant '{tenantId}' y agregado '{typeof(TAggregate).Name}'. " +
                   $"Verifique que RegisterTenantInfraestructure<T>() fue llamado para este tenant."
               );
    }

    public async Task<TAggregate?> GetByIdAsync(TId id, CancellationToken cancellationToken = default) =>
        await GetRepository().GetByIdAsync(id, cancellationToken);

    public async Task<IReadOnlyCollection<TAggregate>> FindAsync(ISpecification<TAggregate> specification, CancellationToken cancellationToken = default) =>
        await GetRepository().FindAsync(specification, cancellationToken);

    public async Task<TAggregate?> FindSingleAsync(ISpecification<TAggregate> specification, CancellationToken cancellationToken = default) =>
        await GetRepository().FindSingleAsync(specification, cancellationToken);

    public async Task AddAsync(TAggregate aggregate, CancellationToken cancellationToken = default) => 
        await GetRepository().AddAsync(aggregate, cancellationToken);

    public void Update(TAggregate aggregate) => 
        GetRepository().Update(aggregate);

    public void Delete(TAggregate aggregate) => 
        GetRepository().Delete(aggregate);
}