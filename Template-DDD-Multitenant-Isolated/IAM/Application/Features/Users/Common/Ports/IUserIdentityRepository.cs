using IAM.Application.Features.Auth.Common.Dtos;

namespace IAM.Application.Features.Users.Common.Ports;

public interface IUserIdentityRepository : IReadIdentityRepository, IWriteIdentityRepository;

public interface IWriteIdentityRepository
{
    Task<(bool IsSuccess, string UserName)> CreateUserAsync(CreateUserDto dto, CancellationToken cancellationToken = default);
    Task<bool> UpdateUser(string userName, string password);
    Task<bool> ChangePassword(string userName, string oldPassword, string newPassword);
    Task<bool> DeleteUser(string userName);
}
public interface IReadIdentityRepository
{
    Task<AuthUserDto?> GetUserAsync(string userName, CancellationToken cancellationToken = default);
}