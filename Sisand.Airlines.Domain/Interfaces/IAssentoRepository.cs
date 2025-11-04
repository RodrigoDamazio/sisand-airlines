using Sisand.Airlines.Domain.Entities;

namespace Sisand.Airlines.Domain.Repositories
{
    public interface IAssentoRepository
    {
        Task<IEnumerable<Assento>> ObterPorVooAsync(Guid vooId);
        Task<Assento?> ObterPorIdAsync(Guid id);
        Task ReservarAssentoAsync(Guid assentoId);
        Task<IEnumerable<Assento>> ObterPorVooIdAsync(Guid vooId);
        Task LiberarAssentoAsync(Guid assentoId);
    }
}
