using Sisand.Airlines.Domain.Entities;

namespace Sisand.Airlines.Domain.Repositories
{
    public interface IAviaoRepository
    {
        Task<IEnumerable<Aviao>> ObterTodosAsync();
    }
}
