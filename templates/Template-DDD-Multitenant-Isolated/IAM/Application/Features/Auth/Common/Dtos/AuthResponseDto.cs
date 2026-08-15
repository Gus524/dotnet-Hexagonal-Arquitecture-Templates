using IAM.Application.Features.Auth.Common.Dtos;

namespace Core.IAM.Application.Features.Auth.Common.Dtos;

public record AuthResponseDto(string AccessToken, string RefreshToken, AuthUserDto User);
