using Dapper;
using Npgsql;
using Sisand.Airlines.Domain.Entities;
using Sisand.Airlines.Domain.Repositories;

namespace Sisand.Airlines.Infrastructure.Repositories
{
    public class VooRepository : IVooRepository
    {
        private readonly NpgsqlConnection _connection;

        public VooRepository(NpgsqlConnection connection)
        {
            _connection = connection;
        }

        public async Task<IEnumerable<Voo>> ObterPorDataAsync(DateTime data)
        {
            const string sql = @"
                SELECT id,
                    airplane_id AS AviaoId,
                    departure_timestamp AS DataPartida,
                    arrival_timestamp AS DataChegada,
                    origin AS Origem,
                    destination AS Destino
                FROM flights
                WHERE departure_timestamp >= @InicioDia
                AND departure_timestamp < @FimDia
                ORDER BY departure_timestamp;";

            var inicioDia = data.Date;
            var fimDia = data.Date.AddDays(1);

            return await _connection.QueryAsync<Voo>(sql, new { InicioDia = inicioDia, FimDia = fimDia });
        }


        public async Task<Voo?> ObterPorIdAsync(Guid id)
        {
            const string sql = @"
                SELECT id, airplane_id AS AviaoId, departure_timestamp AS DataPartida,
                       arrival_timestamp AS DataChegada, origin AS Origem, destination AS Destino
                FROM flights WHERE id = @Id;";

            return await _connection.QueryFirstOrDefaultAsync<Voo>(sql, new { Id = id });
        }

        public async Task<IEnumerable<VooResumo>> ObterPorDataComDisponibilidadeAsync(DateTime data, int minAssentos)
        {
            var sql = @"
                SELECT 
                    f.id, 
                    f.origin AS Origem, 
                    f.destination AS Destino, 
                    f.departure_timestamp AS DataPartida, 
                    f.arrival_timestamp AS DataChegada,
                    COUNT(s.id) FILTER (WHERE s.is_available = true) AS AssentosDisponiveis
                FROM flights f
                JOIN seats s ON s.flight_id = f.id
                WHERE CAST(f.departure_timestamp AS DATE) = @Data
                GROUP BY f.id, f.origin, f.destination, f.departure_timestamp, f.arrival_timestamp
                HAVING COUNT(s.id) FILTER (WHERE s.is_available = true) >= @Min";

            return await _connection.QueryAsync<VooResumo>(sql, new { Data = data.Date, Min = minAssentos });
        }

    }
}
