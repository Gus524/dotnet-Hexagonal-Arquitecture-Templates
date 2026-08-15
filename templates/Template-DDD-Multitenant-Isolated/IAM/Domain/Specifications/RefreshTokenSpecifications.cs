using IAM.Domain.Model;
using SharedKernel.Specification;

namespace IAM.Domain.Specifications;

public class RefreshTokenByHashedTokenSpecification(string tokenHash) : ISpecification<RefreshToken>
{
    public string TokenHash { get; } = tokenHash;

    public bool IsSatisfiedBy(RefreshToken aggregate) => aggregate.TokenHash == TokenHash;
}

public class TokenByUserSpecification(string usuarioId) : ISpecification<RefreshToken>
{
    public string UsuarioId { get; } = usuarioId;

    public bool IsSatisfiedBy(RefreshToken aggregate) => aggregate.UsuarioId == UsuarioId && aggregate.IsActive;
}
