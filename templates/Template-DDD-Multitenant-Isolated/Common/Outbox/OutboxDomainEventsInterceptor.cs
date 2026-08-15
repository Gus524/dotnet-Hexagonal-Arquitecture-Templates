using System.Text.Json;
using Microsoft.EntityFrameworkCore.Diagnostics;
using SharedKernel.Events;
using SharedKernel.Ports.Out.MultiTenancy;

namespace Common.Outbox;

public sealed class OutboxDomainEventsInterceptor(
    ITenantProvider tenantProvider, 
    IDomainEventCollector eventCollector
) : SaveChangesInterceptor
{
    public override async ValueTask<InterceptionResult<int>> SavingChangesAsync(DbContextEventData eventData, InterceptionResult<int> result,
        CancellationToken cancellationToken = new CancellationToken())
    {
        var dbContext = eventData.Context;

        if (dbContext is null) return await base.SavingChangesAsync(eventData, result, cancellationToken);
        
        var aggregates = eventCollector.GetAggregates()
            .Where(x => x.DomainEvents.Count != 0)
            .ToList();
        
        if (aggregates.Count == 0) return await base.SavingChangesAsync(eventData, result, cancellationToken);

        var tenantId = tenantProvider.GetTenantId();
        
        var domainEvents = aggregates
            .SelectMany(x => x.DomainEvents)
            .ToList();

        var storedEvents = domainEvents
            .Select(domainEvent => new StoredEvent(
                domainEvent.EventId,
                domainEvent.GetType().AssemblyQualifiedName!,
                JsonSerializer.Serialize(domainEvent, domainEvent.GetType()),
                tenantId
            )).ToList();
        
        await dbContext.Set<StoredEvent>().AddRangeAsync(storedEvents, cancellationToken);
        
        foreach (var aggregate in aggregates)
        {
            aggregate.ClearDomainEvents();
            eventCollector.Clear();
        }
        
        return await base.SavingChangesAsync(eventData, result, cancellationToken);
    }
}