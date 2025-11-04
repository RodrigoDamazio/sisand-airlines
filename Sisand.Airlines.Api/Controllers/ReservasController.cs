using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Sisand.Airlines.Domain.Interfaces;


namespace Sisand.Airlines.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize] // 🔒 exige token JWT
    public class ReservasController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;

        public ReservasController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        [HttpGet("{usuarioId:guid}")]
        public async Task<IActionResult> ObterPorUsuario(Guid usuarioId)
        {
            try
            {
                var reservas = await _unitOfWork.Reservas.ObterPorUsuarioAsync(usuarioId);

                if (reservas == null || !reservas.Any())
                    return NotFound(new { mensagem = "Nenhuma reserva encontrada." });

                return Ok(reservas);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { erro = ex.Message });
            }
        }

        [HttpGet("minhas")]
        public async Task<IActionResult> MinhasReservas()
        {
            try
            {
                var usuarioIdClaim = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
                if (usuarioIdClaim == null)
                    return Unauthorized(new { erro = "Usuário não autenticado." });

                var usuarioId = Guid.Parse(usuarioIdClaim);

                var reservas = await _unitOfWork.Reservas.ObterPorUsuarioAsync(usuarioId);
                return Ok(reservas);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { erro = ex.Message });
            }
        }


        [HttpDelete("{id}")]
        public async Task<IActionResult> CancelarReserva(Guid id)
        {
            try
            {
                var usuarioIdClaim =
                    User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value ??
                    User.FindFirst("sub")?.Value ??
                    User.FindFirst("id")?.Value;

                if (usuarioIdClaim == null)
                    return Unauthorized(new { erro = "Usuário não autenticado." });

                var usuarioId = Guid.Parse(usuarioIdClaim);

                Console.WriteLine($"🧩 [DEBUG] UsuarioId do token JWT: {usuarioId}");

                await _unitOfWork.BeginTransactionAsync();

                var reserva = await _unitOfWork.Reservas.ObterPorIdAsync(id);
                if (reserva == null)
                    return NotFound(new { erro = "Reserva não encontrada." });

                Console.WriteLine($"🧩 [DEBUG] UsuarioId da reserva: {reserva.UsuarioId}");

                if (reserva.UsuarioId != usuarioId)
                    return Forbid();

                await _unitOfWork.Reservas.RemoverReservaAsync(id);
                await _unitOfWork.Assentos.LiberarAssentoAsync(reserva.AssentoId);
                await _unitOfWork.CommitAsync();

                return Ok(new { mensagem = "Reserva cancelada com sucesso!" });
            }
            catch (Exception ex)
            {
                await _unitOfWork.RollbackAsync();
                return StatusCode(500, new { erro = ex.Message });
            }
        }



        

    }
}
