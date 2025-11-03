namespace Sisand.Airlines.Domain.Entities
{
    public class Aviao
    {
        public Guid Id { get; set; }
        public string Modelo { get; set; } = string.Empty;
        public int CapacidadeEconomica { get; set; }
        public int CapacidadePrimeiraClasse { get; set; }
    }
}
