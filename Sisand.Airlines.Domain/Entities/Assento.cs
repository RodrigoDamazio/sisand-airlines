namespace Sisand.Airlines.Domain.Entities
{
    public enum ClasseAssento
    {
        ECONOMICA,
        PRIMEIRA
    }

    public class Assento
    {
        public Guid Id { get; set; }
        public Guid VooId { get; set; }
        public string Numero { get; set; } = string.Empty;
        public ClasseAssento Classe { get; set; }
        public decimal Preco { get; set; }
        public bool Disponivel { get; set; }  // ✅ Agora bate com "is_available"
    }
}
