using IAM.Domain.Model;
using SharedKernel.Repository;

namespace IAM.Domain.Ports;

public interface IRefreshTokenRepository : IRepository<RefreshToken, RefreshTokenId>;
