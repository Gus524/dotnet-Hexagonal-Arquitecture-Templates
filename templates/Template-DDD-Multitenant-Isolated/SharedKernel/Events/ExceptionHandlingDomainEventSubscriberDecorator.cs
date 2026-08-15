using Microsoft.Extensions.Logging;

namespace SharedKernel.Events;

public class ExceptionHandlingDomainEventSubscriberDecorator<TEvent>(
    IDomainEventSubscriber<TEvent> innerSubscriber,
    ILogger<ExceptionHandlingDomainEventSubscriberDecorator<TEvent>> logger
) : IDomainEventSubscriber<TEvent> where TEvent : IDomainEvent
{
    public async Task On(TEvent @event, CancellationToken cancellationToken = default)
    {
        try
        {
            await innerSubscriber.On(@event, cancellationToken);
        }
        catch (Exception ex)
        {
            logger.LogError(
                ex,
                "Error aislado en subscriptor {SubscriberType} para evento {EventType}",
                innerSubscriber.GetType().Name,
                @event.GetType().Name
            );
            throw;
        }
    }
}