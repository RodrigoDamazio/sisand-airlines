using Microsoft.AspNetCore.Mvc;
using Dapper;
using Sisand.Airlines.Infrastructure.Context;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Sisand.Airlines.Domain.Interfaces;

namespace Sisand.Airlines.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly DapperContext _context;
        private readonly IConfiguration _config;
        private readonly IUnitOfWork _unitOfWork;

        public AuthController(DapperContext context, IConfiguration config, IUnitOfWork unitOfWork)
        {
            _context = context;
            _config = config;
            _unitOfWork = unitOfWork;
        }

        // ✅ LOGIN
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest request)
        {
            try
            {
                var user = await _unitOfWork.Usuarios.ObterPorEmailAsync(request.Email);

                if (user == null)
                    return Unauthorized(new { erro = "Usuário não encontrado." });

                if (!BCrypt.Net.BCrypt.Verify(request.Senha, user.PasswordHash))
                    return Unauthorized(new { erro = "Senha incorreta." });

                // ✅ Aqui garantimos que é um Usuario fortemente tipado
                var token = GerarTokenJwt(user);

                return Ok(new
                {
                    token ,
                    usuarioId = user.Id
                });

            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Erro no login: {ex.Message}");
                Console.WriteLine(ex.StackTrace);
                return StatusCode(500, new { erro = ex.Message });
            }
        }


        // ✅ REGISTER
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterRequest request)
        {
            if (request.Senha != request.ConfirmarSenha)
                return BadRequest("As senhas não coincidem.");

            using var connection = _context.CreateConnection();

            var jaExiste = await connection.QueryFirstOrDefaultAsync<int>(
                "SELECT COUNT(*) FROM users WHERE email = @Email", new { request.Email });

            if (jaExiste > 0)
                return Conflict("E-mail já cadastrado.");

            string hash = BCrypt.Net.BCrypt.HashPassword(request.Senha);

            var userId = Guid.NewGuid();
            await connection.ExecuteAsync(@"
                INSERT INTO users (id, full_name, email, password_hash, cpf, birth_date)
                VALUES (@Id, @Nome, @Email, @PasswordHash, @Cpf, @DataNascimento)",
                new
                {
                    Id = userId,
                    Nome = request.NomeCompleto,
                    Email = request.Email,
                    PasswordHash = hash,
                    Cpf = request.Cpf,
                    DataNascimento = request.DataNascimento
                });

            return Ok(new { mensagem = "Usuário cadastrado com sucesso.", id = userId });
        }

        // 🔑 Função para gerar o token JWT
        private string GerarTokenJwt(Usuario usuario)
        {
            var jwtSection = _config.GetSection("Jwt");
            var key = Encoding.ASCII.GetBytes(jwtSection["Key"]!);

            if (key.Length * 8 < 256)
                throw new Exception("A chave JWT deve ter pelo menos 256 bits (32 caracteres ASCII).");

            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, usuario.Id.ToString()),
                new Claim(JwtRegisteredClaimNames.Email, usuario.Email),
                new Claim(ClaimTypes.NameIdentifier, usuario.Id.ToString()),
                new Claim("nome", usuario.Nome)
            };

            var creds = new SigningCredentials(
                new SymmetricSecurityKey(key),
                SecurityAlgorithms.HmacSha256
            );

            var token = new JwtSecurityToken(
                issuer: jwtSection["Issuer"],
                audience: jwtSection["Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddHours(2),
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }   
}