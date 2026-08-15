using FluentAssertions;
using Prestamos.Domain.Model;
using Xunit;

namespace UnitTests.Domain.Prestamos.Aggregates;

public class PrestatarioTests
{
    [Fact]
    public void Constructor_WithValidIdAndNombre_ShouldInitializePrestatario()
    {
        // Arrange
        var id = new PrestatarioId("EMP-001");
        var nombre = "Juan Pérez";

        // Act
        var prestatario = new Prestatario(id, nombre);

        // Assert
        prestatario.Id.Should().Be(id);
        prestatario.Nombre.Should().Be(nombre);
    }
}
