using System;
using System.Threading.Tasks;
using Franquias.Api.Common;
using Franquias.Api.DTOs;
using Franquias.Api.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Franquias.Api.Controllers
{
    [ApiController]
    [Route("api/vendas")]
    [Authorize]
    public class VendasController : ApiControllerBase
    {
        private readonly IVendaService _vendaService;

        public VendasController(IVendaService vendaService)
        {
            _vendaService = vendaService;
        }

        [HttpGet]
        public async Task<ActionResult> Listar([FromQuery] int? unidadeId, [FromQuery] DateTime? inicio, [FromQuery] DateTime? fim)
        {
            // usuario de unidade so enxerga as vendas da propria unidade
            if (!EhAdministrador && UnidadeDoUsuario.HasValue)
            {
                unidadeId = UnidadeDoUsuario.Value;
            }

            var vendas = await _vendaService.Listar(unidadeId, inicio, fim);
            return Ok(vendas);
        }

        [HttpGet("{id:int}")]
        public async Task<ActionResult<VendaRespostaDto>> ObterPorId(int id)
        {
            var venda = await _vendaService.ObterPorId(id);
            return Ok(venda);
        }

        [HttpPost]
        [Authorize(Roles = Perfis.Todos)]
        public async Task<ActionResult<VendaRespostaDto>> Criar(VendaCriacaoDto dto)
        {
            // usuario de unidade sempre registra venda para a propria unidade
            if (!EhAdministrador && UnidadeDoUsuario.HasValue)
            {
                dto.UnidadeFranqueadaId = UnidadeDoUsuario.Value;
            }

            var venda = await _vendaService.Criar(dto, UsuarioAtualId);
            return CreatedAtAction(nameof(ObterPorId), new { id = venda.Id }, venda);
        }

        [HttpPost("{id:int}/confirmar")]
        [Authorize(Roles = Perfis.Todos)]
        public async Task<ActionResult<VendaRespostaDto>> Confirmar(int id)
        {
            var venda = await _vendaService.Confirmar(id);
            return Ok(venda);
        }

        [HttpPost("{id:int}/cancelar")]
        [Authorize(Roles = Perfis.AdministradorOuGestor)]
        public async Task<ActionResult<VendaRespostaDto>> Cancelar(int id)
        {
            var venda = await _vendaService.Cancelar(id);
            return Ok(venda);
        }
    }
}
