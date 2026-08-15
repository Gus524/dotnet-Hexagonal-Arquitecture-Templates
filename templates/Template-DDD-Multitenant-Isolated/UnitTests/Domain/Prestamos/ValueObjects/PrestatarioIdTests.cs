using FluentAssertions;
using Prestamos.Domain.Model;
using SharedKernel.Exceptions;
using Xunit;

namespace UnitTests.Domain.Prestamos.ValueObjects;

public class PrestatarioIdTests
{
    [Fact]
    public void Constructor_WithValidNoEmpleado_ShouldCreatePrestatarioId()
    {
        // Arrange
        var noEmpleado = "EMP-001";

        // Act
        var id = new PrestatarioId(noEmpleado);

        // Assert
        id.NoEmpleado.Should().Be(noEmpleado);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void Constructor_WithNullOrWhitespaceNoEmpleado_ShouldThrowDomainException(string? invalidNoEmpleado)
    {
        // Arrange & Act
        Action act = () => new PrestatarioId(invalidNoEmpleado!);

        // Assert
        act.Should().Throw<DomainException>()
           .WithMessage("El numero de empleado es obligatorio.");
    }
}
