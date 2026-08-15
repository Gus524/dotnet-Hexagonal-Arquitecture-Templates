using EventsIntegration.Mappers;
using Prestamos.Domain.Model;
using SharedKernel.Events;
using SharedKernel.Ports.In;

namespace EventsIntegration.Handlers;

public class PrestatarioRegistradoIntegrationHandler(IMediator mediator)
    : IDomainEventSubscriber<PrestatarioRegistrado>
{
    public async Task On(PrestatarioRegistrado @event, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(@event);

        var command = PrestatarioToUserCommandMapper.MapToCommand(@event);
        await mediator.Send(command, cancellationToken);
    }
}
