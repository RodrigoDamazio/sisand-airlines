using System.Threading.Tasks;
using Sisand.Airlines.Domain.Repositories;

namespace Sisand.Airlines.Domain.Interfaces
{
    public interface IUnitOfWork
    {
        IVooRepository Voos { get; }
        IAssentoRepository Assentos { get; }
        IReservaRepository Reservas { get; }
        IUsuarioRepository Usuarios { get; }

        Task BeginTransactionAsync();
        Task CommitAsync();
        Task RollbackAsync();
        
    }
}
