using Sisand.Airlines.Domain.Interfaces;
using Sisand.Airlines.Domain.Repositories;
using Sisand.Airlines.Infrastructure.Repositories;

using Sisand.Airlines.Infrastructure.Context;
using Npgsql;


namespace Sisand.Airlines.Infrastructure
{
    public class UnitOfWork : IUnitOfWork, IDisposable
    {
        private readonly DapperContext _context;
        private readonly NpgsqlConnection _connection;
        private NpgsqlTransaction? _transaction;

        public IVooRepository Voos { get; }
        public IAssentoRepository Assentos { get; }
        public IReservaRepository Reservas { get; }
        public IUsuarioRepository Usuarios { get; }

        public UnitOfWork(DapperContext context)
        {
            _context = context;
            _connection = (NpgsqlConnection)_context.CreateConnection();

            // 🔹 Garante que a conexão está aberta antes de repassar aos repositórios
            if (_connection.State != System.Data.ConnectionState.Open)
                _connection.Open();

            Voos = new VooRepository(_connection);
            Assentos = new AssentoRepository(_connection, _context);
            Reservas = new ReservaRepository( _context, _connection);
            Usuarios = new UsuarioRepository(_context);
        }


        public async Task BeginTransactionAsync()
        {
            if (_connection.State != System.Data.ConnectionState.Open)
                await _connection.OpenAsync();

            _transaction = await _connection.BeginTransactionAsync();
        }

        public async Task CommitAsync()
        {
            if (_transaction != null)
                await _transaction.CommitAsync();

            if (_connection.State == System.Data.ConnectionState.Open)
                await _connection.CloseAsync();
        }

        public async Task RollbackAsync()
        {
            try
            {
                if (_transaction != null && _transaction.Connection != null)
                {
                    await _transaction.RollbackAsync();
                }
            }
            catch (InvalidOperationException)
            {
                // Transação já finalizada — apenas ignore.
            }
            finally
            {
                if (_connection.State == System.Data.ConnectionState.Open)
                    await _connection.CloseAsync();
            }
        }


        public void Dispose()
        {
            _transaction?.Dispose();
            _connection.Dispose();
        }
    }
}
