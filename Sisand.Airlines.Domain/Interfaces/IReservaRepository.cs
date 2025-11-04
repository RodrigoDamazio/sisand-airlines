using Sisand.Airlines.Application.DTOs;
using Sisand.Airlines.Domain.Entities;

namespace Sisand.Airlines.Domain.Repositories
{
    public interface IReservaRepository
    {
        Task CriarReservaAsync(Reserva reserva);
        //Task<IEnumerable<object>> ObterPorUsuarioAsync(Guid usuarioId);
        Task<IEnumerable<ReservaDetalhadaDto>> ObterPorUsuarioAsync(Guid usuarioId);
        Task RemoverReservaAsync(Guid id);
        Task<Reserva?> ObterPorIdAsync(Guid id);
        Task<IEnumerable<object>> ObterReservaPorUsuarioAsync(Guid usuarioId);

        
    }
}
