using System;
using System.Threading.Tasks;
using Franquias.Api.Common;
using Franquias.Api.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Franquias.Api.Controllers
{
    [ApiController]
    [Route("api/relatorios")]
    [Authorize(Roles = Perfis.AdministradorOuGestor)]
    public class RelatoriosController : ApiControllerBase
    {
        private readonly IRelatorioService _relatorioService;

        public RelatoriosController(IRelatorioService relatorioService)
        {
            _relatorioService = relatorioService;
        }

        [HttpGet("faturamento")]
        public async Task<ActionResult> Faturamento([FromQuery] DateTime inicio, [FromQuery] DateTime fim)
        {
            var dados = await _relatorioService.FaturamentoPorUnidade(inicio, fim);
            return Ok(dados);
        }

        [HttpGet("ranking-unidades")]
        public async Task<ActionResult> RankingUnidades([FromQuery] DateTime inicio, [FromQuery] DateTime fim)
        {
            var dados = await _relatorioService.RankingUnidades(inicio, fim);
            return Ok(dados);
        }

        [HttpGet("produtos-mais-vendidos")]
        public async Task<ActionResult> ProdutosMaisVendidos([FromQuery] DateTime? inicio, [FromQuery] DateTime? fim,
            [FromQuery] int top = 10)
        {
            var dados = await _relatorioService.ProdutosMaisVendidos(inicio, fim, top);
            return Ok(dados);
        }

        [HttpGet("royalties")]
        public async Task<ActionResult> Royalties([FromQuery] int? ano, [FromQuery] int? mes)
        {
            var dados = await _relatorioService.TotalRoyalties(ano, mes);
            return Ok(dados);
        }

        [HttpGet("estoque-critico")]
        public async Task<ActionResult> EstoqueCritico()
        {
            var dados = await _relatorioService.EstoqueCritico();
            return Ok(dados);
        }

        [HttpGet("chamados-por-status")]
        public async Task<ActionResult> ChamadosPorStatus()
        {
            var dados = await _relatorioService.ChamadosPorStatus();
            return Ok(dados);
        }
    }
}
