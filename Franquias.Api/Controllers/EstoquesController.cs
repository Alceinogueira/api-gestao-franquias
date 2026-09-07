using System.Threading.Tasks;
using Franquias.Api.Common;
using Franquias.Api.DTOs;
using Franquias.Api.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Franquias.Api.Controllers
{
    [ApiController]
    [Route("api/estoques")]
    [Authorize]
    public class EstoquesController : ApiControllerBase
    {
        private readonly IEstoqueService _estoqueService;

        public EstoquesController(IEstoqueService estoqueService)
        {
            _estoqueService = estoqueService;
        }

        [HttpGet("unidade/{unidadeId:int}")]
        public async Task<ActionResult> ListarPorUnidade(int unidadeId)
        {
            var estoques = await _estoqueService.ListarPorUnidade(unidadeId);
            return Ok(estoques);
        }

        [HttpGet("abaixo-do-minimo")]
        public async Task<ActionResult> ItensAbaixoDoMinimo([FromQuery] int? unidadeId)
        {
            var estoques = await _estoqueService.ItensAbaixoDoMinimo(unidadeId);
            return Ok(estoques);
        }

        [HttpGet("{id:int}")]
        public async Task<ActionResult<EstoqueRespostaDto>> ObterPorId(int id)
        {
            var estoque = await _estoqueService.ObterPorId(id);
            return Ok(estoque);
        }

        [HttpGet("{id:int}/movimentacoes")]
        public async Task<ActionResult> ListarMovimentacoes(int id)
        {
            var movimentacoes = await _estoqueService.ListarMovimentacoes(id);
            return Ok(movimentacoes);
        }

        [HttpPost]
        [Authorize(Roles = Perfis.AdministradorOuGestor)]
        public async Task<ActionResult<EstoqueRespostaDto>> Criar(EstoqueCriacaoDto dto)
        {
            var estoque = await _estoqueService.Criar(dto);
            return CreatedAtAction(nameof(ObterPorId), new { id = estoque.Id }, estoque);
        }

        [HttpPost("movimentacoes")]
        [Authorize(Roles = Perfis.AdministradorOuGestor)]
        public async Task<ActionResult<EstoqueRespostaDto>> Movimentar(MovimentacaoEstoqueDto dto)
        {
            var estoque = await _estoqueService.Movimentar(dto);
            return Ok(estoque);
        }
    }
}
