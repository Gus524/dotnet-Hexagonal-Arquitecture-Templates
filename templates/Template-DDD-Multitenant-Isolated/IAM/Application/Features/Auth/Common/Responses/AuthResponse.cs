using Core.IAM.Application.Features.Auth.Common.Dtos;
using IAM.Application.Features.Auth.Common.Dtos;

namespace Core.IAM.Application.Features.Auth.Common.Responses;

public record AuthResponse(string Token, AuthUserDto User);