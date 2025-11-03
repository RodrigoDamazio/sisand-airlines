using Sisand.Airlines.Domain.Entities;

namespace Sisand.Airlines.Domain.Interfaces
{
    public interface IUsuarioRepository
    {
        Task<Usuario?> ObterPorIdAsync(Guid id);
        Task<Usuario?> ObterPorEmailAsync(string email);
        Task CriarAsync(Usuario usuario);

    }
}