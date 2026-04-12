using SharedKernel.Abstractions;
using SharedKernel.Specification;

namespace SharedKernel.Repository;

public interface IRepository<TAggregate, TId> : IReadRepository<TAggregate, TId>, IWriteRepository<TAggregate, TId>
    where TAggregate : class, IAggregateRoot;

public interface IReadRepository<TAggregate, TId> 
    where TAggregate : class, IAggregateRoot
{
    Task<TAggregate?> GetByIdAsync(TId id, CancellationToken cancellationToken = default);
    Task<IReadOnlyCollection<TAggregate>> FindAsync(ISpecification<TAggregate> specification, CancellationToken cancellationToken = default);
    Task<TAggregate?> FindSingleAsync(ISpecification<TAggregate> specification, CancellationToken cancellationToken = default);
}

public interface IWriteRepository<TAggregate, TId> 
    where TAggregate : class, IAggregateRoot
{
    Task AddAsync(TAggregate aggregate, CancellationToken cancellationToken = default);
    void Update(TAggregate aggregate);
    void Delete(TAggregate aggregate);
}

public interface IUnitOfWork
{
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}