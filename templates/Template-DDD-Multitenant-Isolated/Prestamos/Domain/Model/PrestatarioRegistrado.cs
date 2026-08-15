using SharedKernel.Events;

namespace Prestamos.Domain.Model;

public record PrestatarioRegistrado(
    PrestatarioId PrestatarioId,
    string Nombre,
    string Email,
    string UserName
) : DomainEvent;
