using System.Threading.Tasks;
using Franquias.Api.Common;
using Franquias.Api.DTOs;
using Franquias.Api.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Franquias.Api.Controllers
{
    [ApiController]
    [Route("api/unidades")]
    [Authorize]
    public class UnidadesController : ApiControllerBase
    {
        private readonly IUnidadeService _unidadeService;

        public UnidadesController(IUnidadeService unidadeService)
        {
            _unidadeService = unidadeService;
        }

        [HttpGet]
        public async Task<ActionResult> Listar([FromQuery] bool? ativa, [FromQuery] string texto)
        {
            var unidades = await _unidadeService.Listar(ativa, texto);
            return Ok(unidades);
        }

        [HttpGet("{id:int}")]
        public async Task<ActionResult<UnidadeRespostaDto>> ObterPorId(int id)
        {
            var unidade = await _unidadeService.ObterPorId(id);
            return Ok(unidade);
        }

        [HttpPost]
        [Authorize(Roles = Perfis.Administrador)]
        public async Task<ActionResult<UnidadeRespostaDto>> Criar(UnidadeCriacaoDto dto)
        {
            var unidade = await _unidadeService.Criar(dto);
            return CreatedAtAction(nameof(ObterPorId), new { id = unidade.Id }, unidade);
        }

        [HttpPut("{id:int}")]
        [Authorize(Roles = Perfis.Administrador)]
        public async Task<ActionResult<UnidadeRespostaDto>> Atualizar(int id, UnidadeAtualizacaoDto dto)
        {
            var unidade = await _unidadeService.Atualizar(id, dto);
            return Ok(unidade);
        }

        [HttpPatch("{id:int}/situacao")]
        [Authorize(Roles = Perfis.Administrador)]
        public async Task<ActionResult<UnidadeRespostaDto>> AlterarSituacao(int id, AtualizarSituacaoDto dto)
        {
            var unidade = await _unidadeService.AlterarSituacao(id, dto.Ativo);
            return Ok(unidade);
        }

        [HttpPost("{id:int}/responsaveis")]
        [Authorize(Roles = Perfis.Administrador)]
        public async Task<ActionResult<ResponsavelRespostaDto>> AdicionarResponsavel(int id, ResponsavelDto dto)
        {
            var responsavel = await _unidadeService.AdicionarResponsavel(id, dto);
            return Ok(responsavel);
        }
    }
}
