using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace SharedKernel.Events;

public interface IDomainEventDispatcher
{
    Task DispatchAsync(IDomainEvent @event, CancellationToken cancellationToken = default);
    Task DispatchAsync(IEnumerable<IDomainEvent> events, CancellationToken cancellationToken = default);
}

public interface IDomainEventSubscriber<in TEvent> where TEvent : IDomainEvent
{
    Task On(TEvent @event, CancellationToken cancellationToken = default);
}

public class DomainEventDispatcher(
    IServiceProvider serviceProvider,
    ILogger<DomainEventDispatcher>? logger = null
) : IDomainEventDispatcher
{
    public Task DispatchAsync(IDomainEvent @event, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(@event);
        return DispatchAsync([@event], cancellationToken);
    }

    public async Task DispatchAsync(IEnumerable<IDomainEvent> events, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(events);
        var exceptions = new List<Exception>();

        foreach (var @event in events)
        {
            var subscriberType = typeof(IDomainEventSubscriber<>).MakeGenericType(@event.GetType());
            var subscribers = serviceProvider.GetServices(subscriberType);

            foreach (var subscriber in subscribers)
            {
                if (subscriber == null) continue;

                try
                {
                    var method = subscriberType.GetMethod(nameof(IDomainEventSubscriber<>.On));
                    if (method != null)
                    {
                        await (Task)method.Invoke(subscriber, [@event, cancellationToken])!;
                    }
                }
                catch (Exception ex)
                {
                    var actualException = ex is TargetInvocationException targetEx && targetEx.InnerException != null
                        ? targetEx.InnerException
                        : ex;

                    logger?.LogError(
                        actualException,
                        "Error al ejecutar subscriptor {SubscriberType} para evento {EventType}",
                        subscriber.GetType().Name,
                        @event.GetType().Name
                    );

                    exceptions.Add(actualException);
                }
            }
        }

        if (exceptions.Count > 0)
        {
            throw new AggregateException(
                $"Fallo en la ejecución de {exceptions.Count} subscriptor(es) para los eventos de dominio despachados.",
                exceptions);
        }
    }
}