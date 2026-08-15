using Prestamos.Domain.Entities;
using SharedKernel.Repository;

namespace Prestamos.Domain.Ports;

public interface IPrestatarioRepository : IRepository<Prestatario, PrestatarioId>;