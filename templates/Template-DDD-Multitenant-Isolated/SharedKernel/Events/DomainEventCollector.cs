using SharedKernel.Abstractions;

namespace SharedKernel.Events;

public interface IDomainEventCollector
{
    void AddAggregate(IAggregateRoot aggregate);
    IReadOnlyCollection<IAggregateRoot> GetAggregates();
    void Clear();
}

public class DomainEventCollector : IDomainEventCollector
{
    private readonly HashSet<IAggregateRoot> _aggregates = [];

    public void AddAggregate(IAggregateRoot aggregate) => _aggregates.Add(aggregate);
    public IReadOnlyCollection<IAggregateRoot> GetAggregates() => _aggregates.ToList().AsReadOnly();
    public void Clear() => _aggregates.Clear();
}