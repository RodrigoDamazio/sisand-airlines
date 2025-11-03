using Dapper;
using Npgsql;
using Sisand.Airlines.Domain.Entities;
using Sisand.Airlines.Domain.Repositories;
using Sisand.Airlines.Infrastructure.Context;

namespace Sisand.Airlines.Infrastructure.Repositories
{
    public class ReservaRepository : IReservaRepository
    {
        private readonly NpgsqlConnection _connection;

        private readonly DapperContext _context;

        public ReservaRepository(DapperContext context, NpgsqlConnection connection)
        {
            _connection = connection;
            _context = context;

        }
        
        
        public async Task CriarReservaAsync(Reserva reserva)
        {
            const string sql = @"
                INSERT INTO bookings (id, user_id, seat_id, total_price, confirmation_code)
                VALUES (@Id, @UsuarioId, @AssentoId, @ValorTotal, @CodigoConfirmacao);";

            using var conn = _context.CreateConnection();
            await _connection.ExecuteAsync(sql, reserva);
        }

        public async Task<IEnumerable<object>> ObterPorUsuarioAsync(Guid usuarioId)
        {
            using var conn = _context.CreateConnection();

            const string sql = @"
                    SELECT 
                        b.id AS reserva_id,
                        b.confirmation_code AS codigo_confirmacao,
                        b.created_at AS data_reserva,
                        s.seat_number AS numero_assento,
                        s.seat_class AS classe,
                        s.price AS preco,
                        f.origin AS origem,
                        f.destination AS destino,
                        f.departure_timestamp AS data_partida,
                        f.arrival_timestamp AS data_chegada
                    FROM bookings b
                    INNER JOIN seats s ON b.seat_id = s.id
                    INNER JOIN flights f ON s.flight_id = f.id
                    WHERE b.user_id = @UsuarioId
                    ORDER BY f.departure_timestamp DESC;";

            return await conn.QueryAsync<object>(sql, new { UsuarioId = usuarioId });
        }


        
    }
}
