namespace Core.IAM.Application.Features.Auth.Common.Dtos;

public record RefreshTokenDto(string RefreshToken, string HashedToken);
