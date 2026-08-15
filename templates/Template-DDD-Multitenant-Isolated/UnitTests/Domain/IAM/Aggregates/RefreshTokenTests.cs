using FluentAssertions;
using IAM.Domain.Model;
using SharedKernel.Exceptions;
using Xunit;

namespace UnitTests.Domain.IAM.Aggregates;

public class RefreshTokenTests
{
    private const string ValidTokenHash = "hash1234567890abcdef";
    private const string ValidUsuarioId = "usr-001";

    [Fact]
    public void Crear_WithValidParameters_ShouldInstantiateRefreshTokenWithActiveStatus()
    {
        // Arrange
        var beforeCreation = DateTime.UtcNow;

        // Act
        var refreshToken = RefreshToken.Crear(ValidTokenHash, ValidUsuarioId, duracionMeses: 3);

        // Assert
        refreshToken.Should().NotBeNull();
        refreshToken.Id.Should().NotBeNull();
        refreshToken.Id.Value.Should().NotBeEmpty();
        refreshToken.TokenHash.Should().Be(ValidTokenHash);
        refreshToken.UsuarioId.Should().Be(ValidUsuarioId);
        refreshToken.CreatedAt.Should().BeOnOrAfter(beforeCreation).And.BeOnOrBefore(DateTime.UtcNow);
        refreshToken.ExpiresAt.Should().BeAfter(refreshToken.CreatedAt);
        refreshToken.RevokedAt.Should().BeNull();
        refreshToken.IsExpired.Should().BeFalse();
        refreshToken.IsActive.Should().BeTrue();
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void Crear_WithNullOrWhitespaceTokenHash_ShouldThrowDomainException(string? invalidTokenHash)
    {
        // Arrange & Act
        Action act = () => RefreshToken.Crear(invalidTokenHash!, ValidUsuarioId);

        // Assert
        act.Should().Throw<DomainException>()
           .WithMessage("El token hash no puede estar vacío.");
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void Crear_WithNullOrWhitespaceUsuarioId_ShouldThrowDomainException(string? invalidUsuarioId)
    {
        // Arrange & Act
        Action act = () => RefreshToken.Crear(ValidTokenHash, invalidUsuarioId!);

        // Assert
        act.Should().Throw<DomainException>()
           .WithMessage("El identificador del usuario no puede estar vacío.");
    }

    [Fact]
    public void IsExpired_WhenExpiresAtIsInThePast_ShouldReturnTrueAndIsActiveShouldReturnFalse()
    {
        // Act: Passing negative duration creates an already expired token
        var refreshToken = RefreshToken.Crear(ValidTokenHash, ValidUsuarioId, duracionMeses: -1);

        // Assert
        refreshToken.IsExpired.Should().BeTrue();
        refreshToken.IsActive.Should().BeFalse();
    }

    [Fact]
    public void Revocar_WhenTokenIsActive_ShouldSetRevokedAtAndMakeTokenInactive()
    {
        // Arrange
        var refreshToken = RefreshToken.Crear(ValidTokenHash, ValidUsuarioId);
        var beforeRevoke = DateTime.UtcNow;

        // Act
        refreshToken.Revocar();

        // Assert
        refreshToken.RevokedAt.Should().NotBeNull();
        refreshToken.RevokedAt!.Value.Should().BeOnOrAfter(beforeRevoke).And.BeOnOrBefore(DateTime.UtcNow);
        refreshToken.IsActive.Should().BeFalse();
    }

    [Fact]
    public void Revocar_WhenAlreadyRevoked_ShouldNotChangeRevokedAtTimestamp()
    {
        // Arrange
        var refreshToken = RefreshToken.Crear(ValidTokenHash, ValidUsuarioId);
        refreshToken.Revocar();
        var initialRevokedAt = refreshToken.RevokedAt;

        // Act
        refreshToken.Revocar();

        // Assert
        refreshToken.RevokedAt.Should().Be(initialRevokedAt);
    }
}
