using SharedKernel.Abstractions;
using SharedKernel.Exceptions;

namespace IAM.Domain.Model;

public class RefreshToken : AggregateRoot<RefreshTokenId>
{
    public string TokenHash { get; private set; }
    public string UsuarioId { get; private set; }
    public DateTime ExpiresAt { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime? RevokedAt { get; private set; }

    public bool IsActive => RevokedAt == null && !IsExpired;
    public bool IsExpired => ExpiresAt <= DateTime.UtcNow;

    private RefreshToken(RefreshTokenId id, string tokenHash, string usuarioId, DateTime expiresAt) : base(id)
    {
        if (string.IsNullOrWhiteSpace(tokenHash))
            throw new DomainException("El token hash no puede estar vacío.");

        if (string.IsNullOrWhiteSpace(usuarioId))
            throw new DomainException("El identificador del usuario no puede estar vacío.");

        TokenHash = tokenHash;
        UsuarioId = usuarioId;
        CreatedAt = DateTime.UtcNow;
        ExpiresAt = expiresAt;
    }

    public static RefreshToken Crear(string tokenHash, string usuarioId, int duracionMeses = 3)
    {
        var id = RefreshTokenId.New();
        var expiresAt = DateTime.UtcNow.AddMonths(duracionMeses);
        return new RefreshToken(id, tokenHash, usuarioId, expiresAt);
    }

    public void Revocar()
    {
        if (RevokedAt.HasValue) return;
        RevokedAt = DateTime.UtcNow;
    }
}
