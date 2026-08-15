using EventsIntegration.Mappers;
using FluentAssertions;
using Prestamos.Domain.Model;
using SharedKernel.Enums;
using Xunit;

namespace UnitTests.EventsIntegration.Mappers;

public class PrestatarioToUserCommandMapperTests
{
    [Fact]
    public void MapToCommand_ShouldMapPropertiesCorrectly()
    {
        // Arrange
        var prestatarioId = new PrestatarioId("EMP-12345");
        var @event = new PrestatarioRegistrado(prestatarioId, "Juan Perez", "juan.perez@empresa.com", "juanperez");

        // Act
        var command = PrestatarioToUserCommandMapper.MapToCommand(@event);

        // Assert
        command.Should().NotBeNull();
        command.OriginKey.Should().Be("Integration");
        command.UserName.Should().Be("juanperez");
        command.Email.Should().Be("juan.perez@empresa.com");
        command.Password.Should().BeNull();
        command.Rol.Should().Be(Rol.Cliente);
    }

    [Fact]
    public void MapToCommand_ShouldThrowArgumentNullException_WhenEventIsNull()
    {
        // Act
        var act = () => PrestatarioToUserCommandMapper.MapToCommand(null!);

        // Assert
        act.Should().Throw<ArgumentNullException>();
    }
}
