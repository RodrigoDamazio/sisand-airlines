using System.Data;
using Microsoft.Extensions.Configuration;
using Npgsql;


namespace Sisand.Airlines.Infrastructure.Context
{
    public class DapperContext
    {
        private readonly IConfiguration _configuration;
        private readonly string _connectionString;

        public DapperContext(IConfiguration configuration)
        {
            _configuration = configuration;
            _connectionString = _configuration.GetConnectionString("Postgres")
                ?? throw new Exception("String de conexão não encontrada.");
        }

        public IDbConnection CreateConnection()
            => new NpgsqlConnection(_connectionString);
    }
}
