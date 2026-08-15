using SharedKernel.Abstractions;
using SharedKernel.Specification;

namespace SharedKernel.Repository;

public interface IRepository<TAggregate, in TId>
    where TAggregate : AggregateRoot<TId>
    where TId : notnull
{
    Task<TAggregate?> GetByIdAsync(TId id, CancellationToken cancellationToken = default);
    void Add(TAggregate aggregate);
    void Update(TAggregate aggregate);
    void Remove(TAggregate aggregate);
    Task<TAggregate?> FindSingleAsync(ISpecification<TAggregate> spec, CancellationToken cancellationToken = default);
    Task<IEnumerable<TAggregate>> FindAsync(ISpecification<TAggregate> spec,
        CancellationToken cancellationToken = default);
}

public interface IUnitOfWork
{
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}