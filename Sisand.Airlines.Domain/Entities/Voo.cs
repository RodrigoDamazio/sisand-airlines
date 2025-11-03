namespace Sisand.Airlines.Domain.Entities
{
    public class Voo
    {
        public Guid Id { get; set; }
        public Guid AviaoId { get; set; }
        public DateTime DataPartida { get; set; }
        public DateTime DataChegada { get; set; }
        public string Origem { get; set; } = string.Empty;
        public string Destino { get; set; } = string.Empty;
    }
}
