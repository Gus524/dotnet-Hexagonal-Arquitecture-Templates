using IAM.Application.Features.Auth.Common.Responses;
using IAM.Domain.Entities;

namespace IAM.Application.Features.Auth.Common.Services;

public interface ILoginService
{
    Task<AuthResponse?> ProcesarAutenticacion(string userName, string password, Usuario userInfo);
}