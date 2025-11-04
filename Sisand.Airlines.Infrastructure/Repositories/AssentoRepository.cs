using Dapper;
using Npgsql;
using Sisand.Airlines.Domain.Entities;
using Sisand.Airlines.Domain.Repositories;
using Sisand.Airlines.Infrastructure.Context;

namespace Sisand.Airlines.Infrastructure.Repositories
{
    public class AssentoRepository : IAssentoRepository
    {
        private readonly NpgsqlConnection _connection;

        private readonly DapperContext _context;

        public AssentoRepository(NpgsqlConnection connection, DapperContext context)
        {
            _connection = connection;
            _context = context;
        }

        public async Task<IEnumerable<Assento>> ObterPorVooAsync(Guid vooId)
        {
            const string sql = @"
                SELECT 
                    id, 
                    flight_id AS VooId, 
                    seat_number AS Numero,
                    seat_class AS Classe, 
                    price AS Preco,
                    is_available AS Disponivel
                FROM seats 
                WHERE flight_id = @VooId;";

            return await _connection.QueryAsync<Assento>(sql, new { VooId = vooId });
        }

        public async Task LiberarAssentoAsync(Guid assentoId)
        {
            const string sql = "UPDATE seats SET is_available = TRUE WHERE id = @Id;";
            await _connection.ExecuteAsync(sql, new { Id = assentoId });
        }

        public async Task<Assento?> ObterPorIdAsync(Guid id)
        {
            const string sql = @"
                SELECT 
                    id, 
                    flight_id AS VooId, 
                    seat_number AS Numero,
                    seat_class AS Classe, 
                    price AS Preco,
                    is_available AS Disponivel
                FROM seats 
                WHERE id = @Id;";

            return await _connection.QueryFirstOrDefaultAsync<Assento>(sql, new { Id = id });
        }


        public async Task ReservarAssentoAsync(Guid assentoId)
        {
            const string sql = "UPDATE seats SET seat_number = seat_number WHERE id = @Id;";
            await _connection.ExecuteAsync(sql, new { Id = assentoId });
        }

        public async Task<IEnumerable<Assento>> ObterPorVooIdAsync(Guid vooId)
        {
            using var conn = _context.CreateConnection();

            const string sql = @"
                SELECT 
                    id,
                    flight_id AS VooId,
                    seat_number AS Numero,
                    CASE 
                        WHEN seat_class = 'ECONOMICA' THEN 0
                        WHEN seat_class = 'PRIMEIRA' THEN 1
                        ELSE 0
                    END AS Classe,
                    price AS Preco,
                    NOT EXISTS (SELECT 1 FROM bookings b WHERE b.seat_id = s.id) AS Disponivel
                FROM seats s
                WHERE s.flight_id = @VooId
                ORDER BY seat_number;";

            return await conn.QueryAsync<Assento>(sql, new { VooId = vooId });
        }

    }
}
