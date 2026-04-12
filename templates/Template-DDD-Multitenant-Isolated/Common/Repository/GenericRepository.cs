using Microsoft.EntityFrameworkCore;
using SharedKernel.Abstractions;
using SharedKernel.Repository;
using Common.Mappers;
using Common.Specification;
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
    IMapper<TAggregate, TPersistence> mapper
) : IRepository<TAggregate, TId>
    where TAggregate : class, IAggregateRoot
    where TPersistence : class
    where TContext : DbContext
{
    protected readonly TContext Context = dbContext;
    protected readonly DbSet<TPersistence> DbSet = dbContext.Set<TPersistence>();
    protected readonly IMapper<TAggregate, TPersistence> Mapper = mapper;

    /// <summary>
    /// Patrón Template Method: Obliga a la implementación concreta a definir cómo 
    /// se extraen los valores escalares del ValueObject (TId) para que EF Core 
    /// pueda ejecutar el FindAsync con las llaves primarias correctas.
    /// </summary>
    protected abstract object[] ExtractPrimaryKeyValues(TId id);

    public virtual async Task<TAggregate?> GetByIdAsync(TId id, CancellationToken cancellationToken = default)
    {
        var keyValues = ExtractPrimaryKeyValues(id);
        
        var persistenceEntity = await DbSet.FindAsync(keyValues, cancellationToken);
        return persistenceEntity == null ? null : Mapper.Map(persistenceEntity);
    }

    public virtual async Task<IReadOnlyCollection<TAggregate>> FindAsync(
        ISpecification<TAggregate> specification, 
        CancellationToken cancellationToken = default)
    {
        var query = ApplySpecification(specification);
        var persistenceEntities = await query.ToListAsync(cancellationToken);
        
        return persistenceEntities.Select(Mapper.Map).ToList().AsReadOnly();
    }

    public virtual async Task<TAggregate?> FindSingleAsync(
        ISpecification<TAggregate> specification, 
        CancellationToken cancellationToken = default)
    {
        var query = ApplySpecification(specification);
        var persistenceEntity = await query.SingleOrDefaultAsync(cancellationToken);
        
        return persistenceEntity == null ? null : Mapper.Map(persistenceEntity);
    }

    private IQueryable<TPersistence> ApplySpecification(ISpecification<TAggregate> spec)
    {
        IQueryable<TPersistence> query = DbSet;

        // 1. Traducir el Criterio del Dominio a Persistencia usando nuestro ExpressionVisitor
        if (spec.Criteria != null)
        {
            var mapper = new ExpressionMapper<TAggregate, TPersistence>();
            var dataCriteria = mapper.Map(spec.Criteria);
            query = query.Where(dataCriteria);
        }

        // 2. Aplicar Eager Loading solicitados por el dominio
        foreach (var include in spec.Includes)
        {
            var includePath = IncludePathExtractor.GetPath(include);
            query = query.Include(includePath);
        }

        return query;
    }

    public virtual async Task AddAsync(TAggregate aggregate, CancellationToken cancellationToken = default)
    {
        var persistenceEntity = Mapper.Map(aggregate);
        await DbSet.AddAsync(persistenceEntity, cancellationToken);
    }

    public virtual void Update(TAggregate aggregate)
    {
        var persistenceEntity = Mapper.Map(aggregate);
        DbSet.Update(persistenceEntity);
    }

    public virtual void Delete(TAggregate aggregate)
    {
        var persistenceEntity = Mapper.Map(aggregate);
        DbSet.Remove(persistenceEntity);
    }
}