using FluentAssertions;
using IAM.Domain.Model;
using SharedKernel.Enums;
using SharedKernel.Exceptions;
using Xunit;

namespace UnitTests.Domain.IAM.Aggregates;

public class UsuarioTests
{
    private readonly UsuarioId _validId = new("usr-001");
    private const string ValidUserName = "johndoe";
    private const string ValidEmail = "john.doe@example.com";
    private const string ValidNombreCompleto = "John Doe";
    private const Rol ValidRol = Rol.Usuario;

    [Fact]
    public void Create_WithValidParameters_ShouldInstantiateUsuarioWithActiveState()
    {
        // Arrange & Act
        var usuario = Usuario.Create(_validId, ValidUserName, ValidEmail, ValidNombreCompleto, ValidRol);

        // Assert
        usuario.Should().NotBeNull();
        usuario.Id.Should().Be(_validId);
        usuario.UserName.Should().Be(ValidUserName);
        usuario.Email.Should().Be(ValidEmail);
        usuario.NombreCompleto.Should().Be(ValidNombreCompleto);
        usuario.Rol.Should().Be(ValidRol);
        usuario.Estado.Should().Be(EstadoUsuario.Activo);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void Create_WithNullOrWhitespaceNombreCompleto_ShouldThrowDomainException(string? invalidNombreCompleto)
    {
        // Arrange & Act
        Action act = () => Usuario.Create(_validId, ValidUserName, ValidEmail, invalidNombreCompleto!, ValidRol);

        // Assert
        act.Should().Throw<DomainException>()
           .WithMessage("El nombre completo es obligatorio.");
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void Create_WithNullOrWhitespaceNombreUsuario_ShouldThrowDomainException(string? invalidNombreUsuario)
    {
        // Arrange & Act
        Action act = () => Usuario.Create(_validId, invalidNombreUsuario!, ValidEmail, ValidNombreCompleto, ValidRol);

        // Assert
        act.Should().Throw<DomainException>()
           .WithMessage("El nombre de usuario es obligatorio.");
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData("invalidemail.com")]
    [InlineData("noatsign")]
    public void Create_WithInvalidEmail_ShouldThrowDomainException(string? invalidEmail)
    {
        // Arrange & Act
        Action act = () => Usuario.Create(_validId, ValidUserName, invalidEmail!, ValidNombreCompleto, ValidRol);

        // Assert
        act.Should().Throw<DomainException>()
           .WithMessage("El formato del correo electrónico es inválido.");
    }

    [Theory]
    [InlineData("john@example.com", "john@example.com")]
    [InlineData("JOHN@EXAMPLE.COM", "john@example.com")]
    [InlineData("  john@example.com ", "john@example.com")]
    public void Create_WhenUserNameEqualsEmail_ShouldThrowDomainException(string userName, string email)
    {
        // Arrange & Act
        Action act = () => Usuario.Create(_validId, userName, email, ValidNombreCompleto, ValidRol);

        // Assert
        act.Should().Throw<DomainException>()
           .WithMessage("Por seguridad, el nombre de usuario no puede ser igual al correo electrónico.");
    }

    [Fact]
    public void DesactivarUsuario_WhenActive_ShouldChangeEstadoToInactivo()
    {
        // Arrange
        var usuario = Usuario.Create(_validId, ValidUserName, ValidEmail, ValidNombreCompleto, ValidRol);

        // Act
        usuario.DesactivarUsuario();

        // Assert
        usuario.Estado.Should().Be(EstadoUsuario.Inactivo);
    }

    [Fact]
    public void ActivarUsuario_WhenInactivo_ShouldChangeEstadoToActivo()
    {
        // Arrange
        var usuario = Usuario.Create(_validId, ValidUserName, ValidEmail, ValidNombreCompleto, ValidRol);
        usuario.DesactivarUsuario();

        // Act
        usuario.ActivarUsuario();

        // Assert
        usuario.Estado.Should().Be(EstadoUsuario.Activo);
    }
}
