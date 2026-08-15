using FluentAssertions;
using IAM.Domain.Model;
using SharedKernel.Exceptions;
using Xunit;

namespace UnitTests.Domain.IAM.ValueObjects;

public class UsuarioIdTests
{
    [Fact]
    public void Constructor_WithValidOriginKey_ShouldCreateUsuarioId()
    {
        // Arrange
        var originKey = "usr-12345";

        // Act
        var id = new UsuarioId(originKey);

        // Assert
        id.OriginKey.Should().Be(originKey);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void Constructor_WithNullOrWhitespaceOriginKey_ShouldThrowDomainException(string? invalidOriginKey)
    {
        // Arrange & Act
        Action act = () => new UsuarioId(invalidOriginKey!);

        // Assert
        act.Should().Throw<DomainException>()
           .WithMessage("El identificador de origen (OriginKey) no puede ser nulo o vacío.");
    }

    [Fact]
    public void Equality_TwoUsuarioIdsWithSameOriginKey_ShouldBeEqual()
    {
        // Arrange
        var id1 = new UsuarioId("usr-100");
        var id2 = new UsuarioId("usr-100");

        // Act & Assert
        id1.Should().Be(id2);
    }
}
