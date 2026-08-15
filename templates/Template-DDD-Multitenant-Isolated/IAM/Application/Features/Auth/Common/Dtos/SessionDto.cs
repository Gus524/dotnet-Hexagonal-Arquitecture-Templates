using IAM.Application.Features.Auth.Common.Dtos;

namespace Core.IAM.Application.Features.Auth.Common.Dtos;

public record SessionDto(string AccessToken, RefreshTokenDto RefreshTokenDto, AuthUserDto User);
