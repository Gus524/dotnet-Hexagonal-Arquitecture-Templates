using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using Common.Mappers;
using SharedKernel.Abstractions;
using SharedKernel.Events;
using SharedKernel.Repository;
using SharedKernel.Specification;

namespace Common.Repository;

/// <summary>
/// Implementación base de un repositorio de infraestructura para Entity Framework Core.
/// Funciona como un Adaptador Secundario en la Arquitectura Hexagonal, traduciendo 
/// las intenciones del modelo de dominio puro a operaciones de persistencia.
/// </summary>
/// <typeparam name="TAggregate">La Raíz de Agregado del dominio puro.</typeparam>
/// <typeparam name="TId">El identificador fuertemente tipado (Value Object) del Agregado.</typeparam>
/// <typeparam name="TPersistence">El modelo de datos (Data Model) acoplado a Entity Framework.</typeparam>
/// <typeparam name="TContext">El contexto transaccional multitenant.</typeparam>
public abstract class GenericRepository<TAggregate, TId, TPersistence, TContext>(
    TContext dbContext,
    IMapper<TAggregate, TPersistence> mapper,
    IDomainEventCollector eventCollector
) : IRepository<TAggregate, TId>
    where TAggregate : AggregateRoot<TId>
    where TPersistence : class
    where TContext : DbContext
    where TId : notnull
{
    private readonly DbSet<TPersistence> _dbSet = dbContext.Set<TPersistence>();

    /// <summary>
    /// Patrón Template Method: Obliga a la implementación concreta a definir cómo 
    /// se extraen los valores escalares del ValueObject (TId) para que EF Core 
    /// pueda ejecutar el FindAsync con las llaves primarias correctas.
    /// </summary>
    protected abstract object[] ExtractPrimaryKeyValues(TId id);

    protected virtual IQueryable<TPersistence> OnConfigureHydration(IQueryable<TPersistence> query) => query;
    protected abstract Expression<Func<TPersistence, bool>> FilterById(TId id);

    public virtual async Task<TAggregate?> GetByIdAsync(TId id, CancellationToken cancellationToken = default)
    {
        IQueryable<TPersistence> query = _dbSet.AsQueryable();
        
        query = OnConfigureHydration(query);
        
        var persistence = await query 
            .FirstOrDefaultAsync(FilterById(id), cancellationToken);
        
        return persistence == null ? null : mapper.MapToDomain(persistence);
    }

    public virtual void Add(TAggregate aggregate)
    {
        eventCollector.AddAggregate(aggregate);
        var persistence = mapper.MapToPersistence(aggregate);
        _dbSet.Add(persistence);
    }

    public virtual void Update(TAggregate aggregate)
    {
        eventCollector.AddAggregate(aggregate);
        var key = ExtractPrimaryKeyValues(aggregate.Id);
        var persistenceEntity = _dbSet.Find(key);

        if (persistenceEntity == null)
            throw new InvalidOperationException(
                $"No se pudo encontrar la entidad de persistencia para el Agregado {typeof(TAggregate).Name} con ID {aggregate.Id}");

        mapper.MapToExistingPersistence(aggregate, persistenceEntity);
    }

    public virtual void Remove(TAggregate aggregate)
    {
        eventCollector.AddAggregate(aggregate);
        var key = ExtractPrimaryKeyValues(aggregate.Id);
        
        var persistenceEntity = _dbSet.Find(key);
        if (persistenceEntity != null)
        {
            dbContext.Set<TPersistence>().Remove(persistenceEntity);
        }
    }

    public virtual async Task<TAggregate?> FindSingleAsync(ISpecification<TAggregate> spec,
        CancellationToken cancellationToken = default)
    {
        var query = BuildSpecificationQuery(spec);
        var persistence = await query.SingleOrDefaultAsync(cancellationToken);

        return persistence == null ? null : mapper.MapToDomain(persistence);
    }

    public virtual async Task<IEnumerable<TAggregate>> FindAsync(ISpecification<TAggregate> spec,
        CancellationToken cancellationToken = default)
    {
       var query = BuildSpecificationQuery(spec);
        var results = await query.ToListAsync(cancellationToken);

        return results.Select(mapper.MapToDomain);
    }

    private IQueryable<TPersistence> BuildSpecificationQuery(ISpecification<TAggregate> spec)
    {
        var query = _dbSet.AsQueryable();
        query = OnConfigureHydration(query);

        query = ApplySpecification(query, spec);

        return query;
    }

    protected virtual IQueryable<TPersistence> ApplySpecification(IQueryable<TPersistence> query,
        ISpecification<TAggregate> spec) => query;
}