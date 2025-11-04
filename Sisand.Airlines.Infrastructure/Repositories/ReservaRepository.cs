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
                VALUES (@Id, @UserId, @SeatId, @TotalPrice, @ConfirmationCode);";

            using var conn = _context.CreateConnection();
            Console.WriteLine($"[DEBUG] Inserindo reserva - UsuarioId: {reserva.UsuarioId}");
            await conn.ExecuteAsync(sql, new
            {
                Id = reserva.Id,
                UserId = reserva.UsuarioId,
                SeatId = reserva.AssentoId,
                TotalPrice = reserva.ValorTotal,
                ConfirmationCode = reserva.CodigoConfirmacao
            });
        }


        public async Task RemoverReservaAsync(Guid id)
        {
            const string sql = "DELETE FROM bookings WHERE id = @Id;";
            await _connection.ExecuteAsync(sql, new { Id = id });
        }

        public async Task<IEnumerable<object>> ObterPorUsuarioAsync(Guid usuarioId)
        {
              const string sql = @"
                                SELECT 
                                    r.id,
                                    r.confirmation_code AS CodigoConfirmacao,
                                    f.origin AS Origem,
                                    f.destination AS Destino,
                                    s.seat_number AS Assento,
                                    r.total_price AS ValorTotal,
                                    f.departure_timestamp AS DataPartida,
                                    f.arrival_timestamp AS DataChegada
                                FROM bookings r
                                INNER JOIN seats s ON r.seat_id = s.id
                                INNER JOIN flights f ON s.flight_id = f.id
                                WHERE r.user_id = @UsuarioId
                                ORDER BY f.departure_timestamp DESC;";                  

            return await _connection.QueryAsync<object>(sql, new { UsuarioId = usuarioId });
        }

        public async Task<Reserva?> ObterPorIdAsync(Guid id)
        {
            const string sql = @"
                SELECT 
                    id,
                    user_id AS UsuarioId,
                    seat_id AS AssentoId,
                    total_price AS ValorTotal,
                    confirmation_code AS CodigoConfirmacao
                FROM bookings
                WHERE id = @Id;";

            using var conn = _context.CreateConnection();
            return await conn.QueryFirstOrDefaultAsync<Reserva>(sql, new { Id = id });
        }


        public async Task<IEnumerable<object>> ObterReservaPorUsuarioAsync(Guid usuarioId)
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
