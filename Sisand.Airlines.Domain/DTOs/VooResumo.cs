public class VooResumo
{
    public Guid Id { get; set; }
    public string Origem { get; set; } = string.Empty;
    public string Destino { get; set; } = string.Empty;
    public DateTime DataPartida { get; set; }
    public DateTime DataChegada { get; set; }
    public int AssentosDisponiveis { get; set; }
}
