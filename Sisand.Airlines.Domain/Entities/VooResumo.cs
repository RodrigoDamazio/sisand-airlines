namespace Sisand.Airlines.Domain.Entities
{
    public class VooResumo
    {
        public Guid Id { get; set; }
        public string Origem { get; set; } = string.Empty;
        public string Destino { get; set; } = string.Empty;
        public DateTime DataPartida { get; set; }
        public decimal Valor { get; set; }
    }
}
