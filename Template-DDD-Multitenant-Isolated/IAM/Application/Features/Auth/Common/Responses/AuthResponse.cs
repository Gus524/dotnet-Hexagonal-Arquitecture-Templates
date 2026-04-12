using IAM.Application.Features.Auth.Common.Dtos;

namespace IAM.Application.Features.Auth.Common.Responses;

public record AuthResponse(string Token, AuthUserDto User);