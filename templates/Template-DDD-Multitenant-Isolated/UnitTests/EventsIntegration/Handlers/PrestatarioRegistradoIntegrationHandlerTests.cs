using EventsIntegration.Handlers;
using FluentAssertions;
using IAM.Application.Features.Users.Commands.CreateUser;
using NSubstitute;
using Prestamos.Domain.Model;
using SharedKernel.Ports.In;
using SharedKernel.Wrappers;
using Xunit;

namespace UnitTests.EventsIntegration.Handlers;

public class PrestatarioRegistradoIntegrationHandlerTests
{
    [Fact]
    public async Task On_ShouldDispatchCreateUserCommand_ViaMediator()
    {
        // Arrange
        var mediator = Substitute.For<IMediator>();
        mediator.Send(Arg.Any<CreateUserCommand>(), Arg.Any<CancellationToken>())
            .Returns(Task.FromResult(Response.Success("juanperez")));

        var handler = new PrestatarioRegistradoIntegrationHandler(mediator);
        var @event = new PrestatarioRegistrado(
            new PrestatarioId("EMP-99"),
            "Carlos Lopez",
            "carlos.lopez@empresa.com",
            "clopez"
        );

        // Act
        await handler.On(@event, CancellationToken.None);

        // Assert
        await mediator.Received(1).Send(
            Arg.Is<CreateUserCommand>(c =>
                c.UserName == "clopez" &&
                c.Email == "carlos.lopez@empresa.com" &&
                c.Password == null &&
                c.OriginKey == "Integration"
            ),
            Arg.Any<CancellationToken>()
        );
    }

    [Fact]
    public void On_ShouldThrowArgumentNullException_WhenEventIsNull()
    {
        // Arrange
        var mediator = Substitute.For<IMediator>();
        var handler = new PrestatarioRegistradoIntegrationHandler(mediator);

        // Act
        var act = () => handler.On(null!, CancellationToken.None);

        // Assert
        act.Should().ThrowAsync<ArgumentNullException>();
    }
}
