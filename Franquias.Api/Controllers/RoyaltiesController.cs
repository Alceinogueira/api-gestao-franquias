using System.Threading.Tasks;
using Franquias.Api.Common;
using Franquias.Api.DTOs;
using Franquias.Api.Models;
using Franquias.Api.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Franquias.Api.Controllers
{
    [ApiController]
    [Route("api/royalties")]
    [Authorize]
    public class RoyaltiesController : ApiControllerBase
    {
        private readonly IRoyaltyService _royaltyService;

        public RoyaltiesController(IRoyaltyService royaltyService)
        {
            _royaltyService = royaltyService;
        }

        [HttpGet]
        [Authorize(Roles = Perfis.AdministradorOuGestor)]
        public async Task<ActionResult> Listar([FromQuery] int? unidadeId, [FromQuery] int? ano,
            [FromQuery] int? mes, [FromQuery] StatusPagamento? status)
        {
            if (!EhAdministrador && UnidadeDoUsuario.HasValue)
            {
                unidadeId = UnidadeDoUsuario.Value;
            }

            var royalties = await _royaltyService.Listar(unidadeId, ano, mes, status);
            return Ok(royalties);
        }

        [HttpGet("{id:int}")]
        [Authorize(Roles = Perfis.AdministradorOuGestor)]
        public async Task<ActionResult<RoyaltyRespostaDto>> ObterPorId(int id)
        {
            var royalty = await _royaltyService.ObterPorId(id);
            return Ok(royalty);
        }

        [HttpPost("gerar")]
        [Authorize(Roles = Perfis.Administrador)]
        public async Task<ActionResult<RoyaltyRespostaDto>> Gerar(GerarRoyaltyDto dto)
        {
            var royalty = await _royaltyService.Gerar(dto);
            return CreatedAtAction(nameof(ObterPorId), new { id = royalty.Id }, royalty);
        }

        [HttpPatch("{id:int}/pagamento")]
        [Authorize(Roles = Perfis.Administrador)]
        public async Task<ActionResult<RoyaltyRespostaDto>> RegistrarPagamento(int id)
        {
            var royalty = await _royaltyService.RegistrarPagamento(id);
            return Ok(royalty);
        }
    }
}
