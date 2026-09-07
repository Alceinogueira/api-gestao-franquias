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
    [Route("api/chamados")]
    [Authorize]
    public class ChamadosController : ApiControllerBase
    {
        private readonly IChamadoService _chamadoService;

        public ChamadosController(IChamadoService chamadoService)
        {
            _chamadoService = chamadoService;
        }

        [HttpGet]
        public async Task<ActionResult> Listar([FromQuery] int? unidadeId, [FromQuery] StatusChamado? status,
            [FromQuery] PrioridadeChamado? prioridade)
        {
            if (!EhAdministrador && UnidadeDoUsuario.HasValue)
            {
                unidadeId = UnidadeDoUsuario.Value;
            }

            var chamados = await _chamadoService.Listar(unidadeId, status, prioridade);
            return Ok(chamados);
        }

        [HttpGet("{id:int}")]
        public async Task<ActionResult<ChamadoRespostaDto>> ObterPorId(int id)
        {
            var chamado = await _chamadoService.ObterPorId(id);
            return Ok(chamado);
        }

        [HttpPost]
        [Authorize(Roles = Perfis.Todos)]
        public async Task<ActionResult<ChamadoRespostaDto>> Criar(ChamadoCriacaoDto dto)
        {
            if (!EhAdministrador && UnidadeDoUsuario.HasValue)
            {
                dto.UnidadeFranqueadaId = UnidadeDoUsuario.Value;
            }

            var chamado = await _chamadoService.Criar(dto);
            return CreatedAtAction(nameof(ObterPorId), new { id = chamado.Id }, chamado);
        }

        [HttpPut("{id:int}")]
        [Authorize(Roles = Perfis.AdministradorOuGestor)]
        public async Task<ActionResult<ChamadoRespostaDto>> Atualizar(int id, ChamadoAtualizacaoDto dto)
        {
            var chamado = await _chamadoService.Atualizar(id, dto);
            return Ok(chamado);
        }
    }
}
