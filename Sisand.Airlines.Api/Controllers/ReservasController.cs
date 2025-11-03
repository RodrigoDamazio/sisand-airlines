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
    }
}
