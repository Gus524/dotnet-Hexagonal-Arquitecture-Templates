using IAM.Domain.Model;
using SharedKernel.Repository;

namespace IAM.Domain.Ports;

public interface IUsuarioRepository : IRepository<Usuario, UsuarioId>
{
    Task AddAsync(Usuario usuario, CancellationToken cancellationToken = default);
}
