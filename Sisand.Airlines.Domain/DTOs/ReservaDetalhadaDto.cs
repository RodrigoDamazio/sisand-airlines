namespace Sisand.Airlines.Application.DTOs
{
    public class ReservaDetalhadaDto
    {
        public Guid Id { get; set; }
        public string CodigoConfirmacao { get; set; } = string.Empty;
        public string Origem { get; set; } = string.Empty;
        public string Destino { get; set; } = string.Empty;
        public string Assento { get; set; } = string.Empty;
        public decimal ValorTotal { get; set; }
        public DateTime DataPartida { get; set; }
    }
}
