using Microsoft.AspNetCore.Mvc;
using Sisand.Airlines.Domain.Interfaces;

namespace Sisand.Airlines.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class VoosController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;

        public VoosController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        // 🔹 Teste de conexão simples (opcional)
        [HttpGet("pingdb")]
        public IActionResult PingDb()
        {
            return Ok(new { status = "Conexão operacional via UnitOfWork!" });
        }

        // 🔹 Buscar voos disponíveis por data
        [HttpGet]
        public async Task<IActionResult> GetVoos([FromQuery] DateTime? data, [FromQuery] int minAssentos = 1)
        {
            var dataConsulta = data?.Date ?? DateTime.UtcNow.Date;

            // Novo método que conta assentos livres por voo
            var voos = await _unitOfWork.Voos.ObterPorDataComDisponibilidadeAsync(dataConsulta, minAssentos);

            if (!voos.Any())
                return NotFound(new { mensagem = "Nenhum voo disponível na data informada." });

            return Ok(voos);
        }


        // 🔹 Listar assentos de um voo
        [HttpGet("{vooId}/assentos")]
        public async Task<IActionResult> GetAssentos(Guid vooId)
        {
            var assentos = await _unitOfWork.Assentos.ObterPorVooAsync(vooId);

            if (!assentos.Any())
                return NotFound(new { mensagem = "Nenhum assento encontrado para este voo." });

            return Ok(assentos);
        }
    }
}
