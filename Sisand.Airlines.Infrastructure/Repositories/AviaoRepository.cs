using Dapper;
using Npgsql;
using Sisand.Airlines.Domain.Entities;
using Sisand.Airlines.Domain.Repositories;

namespace Sisand.Airlines.Infrastructure.Repositories
{
    public class AviaoRepository : IAviaoRepository
    {
        private readonly NpgsqlConnection _connection;

        public AviaoRepository(NpgsqlConnection connection)
        {
            _connection = connection;
        }

        public async Task<IEnumerable<Aviao>> ObterTodosAsync()
        {
            var sql = "SELECT id, modelo, capacidadeeconomica, capacidadeprimeiraclasse FROM avioes";
            return await _connection.QueryAsync<Aviao>(sql);
        }
    }
}
