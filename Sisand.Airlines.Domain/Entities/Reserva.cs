namespace Sisand.Airlines.Domain.Entities
{
    public class Reserva
    {
        public Guid Id { get; set; }
        public Guid UsuarioId { get; set; }
        public Guid AssentoId { get; set; }
        public DateTime DataReserva { get; set; } = DateTime.UtcNow;
        public decimal ValorTotal { get; set; }
        public string CodigoConfirmacao { get; set; } = string.Empty;
    }
}
