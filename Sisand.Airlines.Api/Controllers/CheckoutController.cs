using Microsoft.AspNetCore.Mvc;
using Sisand.Airlines.Domain.Entities;
using Sisand.Airlines.Domain.Interfaces;
using System.Net.Mail;

namespace Sisand.Airlines.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    // [Authorize] // TEMP: Autenticação desativada para execução do teste de aptidão
    public class CheckoutController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IConfiguration _config;

        public CheckoutController(IUnitOfWork unitOfWork, IConfiguration config)
        {
            _unitOfWork = unitOfWork;
            _config = config;
        }

        [HttpPost("reservar")]
        public async Task<IActionResult> Reservar([FromBody] ReservaRequest req)
        {
            try
            {
                // 🧩 Pega o ID do usuário logado do token JWT
                var usuarioIdClaim = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
                if (usuarioIdClaim == null)
                    return Unauthorized(new { erro = "Usuário não autenticado." });

                var usuarioId = Guid.Parse(usuarioIdClaim);

                // 🔎 Buscar usuário
                var usuario = await _unitOfWork.Usuarios.ObterPorIdAsync(usuarioId);
                if (usuario == null)
                    return NotFound(new { erro = "Usuário não encontrado." });

                if (string.IsNullOrWhiteSpace(usuario.Email))
                    return BadRequest(new { erro = "Usuário sem e-mail cadastrado." });

                // 🔎 Validar assento
                var assento = await _unitOfWork.Assentos.ObterPorIdAsync(req.AssentoId);
                if (assento == null)
                    return NotFound(new { erro = "Assento não encontrado." });

                if (!assento.Disponivel)
                    return BadRequest(new { erro = "Assento já está reservado." });


                // 🪪 Criar reserva
                var reserva = new Reserva
                {
                    Id = Guid.NewGuid(),
                    UsuarioId = usuario.Id,
                    AssentoId = req.AssentoId,
                    ValorTotal = assento.Preco,
                    CodigoConfirmacao = $"SIS-{DateTime.UtcNow:yyyyMMddHHmmss}-{new Random().Next(1000, 9999)}"
                };

                await _unitOfWork.BeginTransactionAsync();
                await _unitOfWork.Reservas.CriarReservaAsync(reserva);
                await _unitOfWork.CommitAsync();

                // ✉️ Enviar e-mail de confirmação
                var voo = await _unitOfWork.Voos.ObterPorIdAsync(assento.VooId);
                if (voo != null)
                {
                    await EnviarEmailConfirmacao(
                        usuario.Email,
                        reserva.CodigoConfirmacao,
                        voo.Origem,
                        voo.Destino,
                        voo.DataPartida,
                        voo.DataChegada
                    );
                }

                return Ok(new
                {
                    mensagem = "Reserva confirmada com sucesso!",
                    codigoConfirmacao = reserva.CodigoConfirmacao,
                    valorTotal = assento.Preco
                });
            }
            catch (Exception ex)
            {
                await _unitOfWork.RollbackAsync();
                return StatusCode(500, new { erro = ex.Message });
            }
        }

        [HttpGet("reservas/{usuarioId:guid}")]
        public async Task<IActionResult> ObterReservasPorUsuario(Guid usuarioId)
        {
            try
            {
                var reservas = await _unitOfWork.Reservas.ObterPorUsuarioAsync(usuarioId);

                if (reservas == null || !reservas.Any())
                    return NotFound(new { mensagem = "Nenhuma reserva encontrada para este usuário." });

                return Ok(reservas);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { erro = ex.Message });
            }
        }



        private async Task EnviarEmailConfirmacao(
            string email,
            string codigo,
            string origem,
            string destino,
            DateTime partida,
            DateTime chegada)
        {
            var smtpSection = _config.GetSection("Smtp");
            string host = smtpSection["Host"] ?? "localhost";
            int port = int.Parse(smtpSection["Port"] ?? "1025");
            string from = smtpSection["From"] ?? "reservas@sisandairlines.com";

            using var client = new SmtpClient(host, port)
            {
                EnableSsl = false,
                DeliveryMethod = SmtpDeliveryMethod.Network
            };

            var subject = "✈️ Confirmação de Reserva - Sisand Airlines";

            var body = $@"
                <!DOCTYPE html>
                <html lang='pt-BR'>
                <head>
                    <meta charset='UTF-8'>
                    <style>
                        body {{
                            font-family: 'Segoe UI', Arial, sans-serif;
                            background-color: #f6f8fb;
                            margin: 0;
                            padding: 40px;
                        }}
                        .container {{
                            background: white;
                            border-radius: 12px;
                            box-shadow: 0 2px 8px rgba(0,0,0,0.1);
                            padding: 30px;
                            max-width: 600px;
                            margin: auto;
                        }}
                        h2 {{
                            color: #005eb8;
                            text-align: center;
                            margin-bottom: 30px;
                        }}
                        .info {{
                            color: #333;
                            font-size: 16px;
                            line-height: 1.6;
                        }}
                        .codigo {{
                            background-color: #005eb8;
                            color: white;
                            padding: 12px;
                            border-radius: 8px;
                            text-align: center;
                            font-weight: bold;
                            font-size: 18px;
                            margin: 20px 0;
                        }}
                        .footer {{
                            text-align: center;
                            color: #999;
                            font-size: 13px;
                            margin-top: 30px;
                        }}
                    </style>
                </head>
                <body>
                    <div class='container'>
                        <h2>Confirmação de Reserva - Sisand Airlines</h2>
                        <p class='info'>Olá! Sua reserva foi confirmada com sucesso.</p>
                        <p class='info'>
                            <strong>🛫 Origem:</strong> {origem}<br>
                            <strong>🏁 Destino:</strong> {destino}<br>
                            <strong>📅 Embarque:</strong> {partida:dd/MM/yyyy HH:mm}<br>
                            <strong>🕘 Chegada:</strong> {chegada:dd/MM/yyyy HH:mm}
                        </p>
                        <div class='codigo'>Código de Confirmação: {codigo}</div>
                        <p class='info'>Obrigado por escolher a Sisand Airlines! 🛩️</p>
                        <div class='footer'>© {DateTime.Now.Year} Sisand Airlines - Todos os direitos reservados.</div>
                    </div>
                </body>
                </html>";

            var mail = new MailMessage(from, email)
            {
                Subject = subject,
                Body = body,
                IsBodyHtml = true
            };

            await client.SendMailAsync(mail);
        }

        public class ReservaRequest
        {
            public Guid UsuarioId { get; set; }
            public Guid AssentoId { get; set; }
            public string UsuarioEmail { get; set; } = string.Empty;
        }
    }
}
