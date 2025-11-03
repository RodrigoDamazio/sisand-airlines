using Dapper;
using Sisand.Airlines.Domain.Entities;
using Sisand.Airlines.Domain.Interfaces;
using Sisand.Airlines.Infrastructure.Context;

namespace Sisand.Airlines.Infrastructure.Repositories
{
    public class UsuarioRepository : IUsuarioRepository
    {
        private readonly DapperContext _context;

        public UsuarioRepository(DapperContext context)
        {
            _context = context;
        }

        public async Task<Usuario?> ObterPorIdAsync(Guid id)
        {
            using var conn = _context.CreateConnection();
            return await conn.QueryFirstOrDefaultAsync<Usuario>(
                "SELECT id, full_name AS Nome, email, cpf, password_hash AS PasswordHash FROM users WHERE id = @id",
                new { id });
        }

        public async Task<Usuario?> ObterPorEmailAsync(string email)
        {
            using var conn = _context.CreateConnection();
            return await conn.QueryFirstOrDefaultAsync<Usuario>(
                "SELECT id, full_name AS Nome, email, cpf, password_hash AS PasswordHash FROM users WHERE email = @email",
                new { email });
        }

        public async Task CriarAsync(Usuario usuario)
        {
            using var conn = _context.CreateConnection();
            await conn.ExecuteAsync(@"
                INSERT INTO users (id, full_name, email, cpf, password_hash)
                VALUES (@Id, @Nome, @Email, @Cpf, @PasswordHash)
            ", usuario);
        }
    }
}
