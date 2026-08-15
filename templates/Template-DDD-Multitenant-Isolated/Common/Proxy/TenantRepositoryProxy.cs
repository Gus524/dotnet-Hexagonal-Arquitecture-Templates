using Microsoft.Extensions.DependencyInjection;
using SharedKernel.Abstractions;
using SharedKernel.Ports.Out.MultiTenancy;
using SharedKernel.Repository;
using SharedKernel.Specification;

namespace Common.Proxy;

/// <summary>
/// Actúa como un enrutador dinámico de persistencia en arquitecturas multitenancy, interceptando 
/// de forma transparente las operaciones de datos para redirigirlas al repositorio del inquilino activo.
/// </summary>
/// <typeparam name="TAggregate">La Raíz de Agregado del dominio puro.</typeparam>
/// <typeparam name="TId">El identificador fuertemente tipado (Value Object o primitivo) del Agregado.</typeparam>
public class TenantRepositoryProxy<TAggregate, TId>(
    ITenantProvider tenantProvider,
    IServiceProvider serviceProvider) : IRepository<TAggregate, TId> 
    where TAggregate : AggregateRoot<TId>
    where TId : notnull
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
                   $"Error Arquitectónico: No se encontró un repositorio registrado para el tenant '{tenantId}' y el Aggregate '{typeof(TAggregate).Name}'." +
                   $"Verifique que ReggisterTenantInfraestructure<T>() fue llamado para este tenant."
               );
    }

    public async Task<TAggregate?> GetByIdAsync(TId id, CancellationToken cancellationToken = default) =>
        await GetRepository().GetByIdAsync(id, cancellationToken);
    public void Add(TAggregate aggregate) => GetRepository().Add(aggregate);
    public void Update(TAggregate aggregate) => GetRepository().Update(aggregate);
    public void Remove(TAggregate aggregate) => GetRepository().Remove(aggregate);
    public Task<TAggregate?> FindSingleAsync(ISpecification<TAggregate> spec,
        CancellationToken cancellationToken = default) => GetRepository().FindSingleAsync(spec, cancellationToken);
    public Task<IEnumerable<TAggregate>> FindAsync(ISpecification<TAggregate> spec,
        CancellationToken cancellationToken = default) => GetRepository().FindAsync(spec, cancellationToken);
}