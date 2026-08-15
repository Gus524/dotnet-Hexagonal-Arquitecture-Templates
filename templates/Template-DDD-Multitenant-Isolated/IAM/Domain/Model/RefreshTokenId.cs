namespace IAM.Domain.Model;

public record RefreshTokenId(Guid Value)
{
    public static RefreshTokenId New() => new(Guid.NewGuid());
}
