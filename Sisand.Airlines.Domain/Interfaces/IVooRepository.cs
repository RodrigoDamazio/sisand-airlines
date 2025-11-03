using Sisand.Airlines.Domain.Entities;

namespace Sisand.Airlines.Domain.Repositories
{
    public interface IVooRepository
    {
        Task<IEnumerable<Voo>> ObterPorDataAsync(DateTime data);
        Task<Voo?> ObterPorIdAsync(Guid id);
        Task<IEnumerable<VooResumo>> ObterPorDataComDisponibilidadeAsync(DateTime data, int minAssentos);
    }
}
