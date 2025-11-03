public class Usuario
{
    public Guid Id { get; set; }
    public string Nome { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Cpf { get; set; } = string.Empty;
    public string PasswordHash { get; set; } = string.Empty;
    public DateTime DataNascimento { get; set; } 
}
