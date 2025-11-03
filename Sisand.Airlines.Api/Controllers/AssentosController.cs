using Microsoft.AspNetCore.Mvc;
using Sisand.Airlines.Domain.Interfaces;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Sisand.Airlines.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AssentosController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;

        public AssentosController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        [HttpGet("{vooId}")]
        public async Task<IActionResult> GetAssentosPorVoo(Guid vooId)
        {
            try
            {
                var assentos = await _unitOfWork.Assentos.ObterPorVooIdAsync(vooId);

                if (assentos == null || !assentos.Any())
                    return NotFound(new { mensagem = "Nenhum assento encontrado para este voo." });

                return Ok(assentos);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Erro ao buscar assentos: {ex.Message}");
                return StatusCode(500, new { erro = "Erro interno ao buscar assentos." });
            }
        }
    }
}
