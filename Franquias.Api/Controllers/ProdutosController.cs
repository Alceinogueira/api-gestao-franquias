using System.Threading.Tasks;
using Franquias.Api.Common;
using Franquias.Api.DTOs;
using Franquias.Api.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Franquias.Api.Controllers
{
    [ApiController]
    [Route("api/produtos")]
    [Authorize]
    public class ProdutosController : ApiControllerBase
    {
        private readonly IProdutoService _produtoService;

        public ProdutosController(IProdutoService produtoService)
        {
            _produtoService = produtoService;
        }

        [HttpGet]
        public async Task<ActionResult> Listar([FromQuery] string nome, [FromQuery] int? categoriaId,
            [FromQuery] bool? ativo, [FromQuery] bool? ehServico)
        {
            var produtos = await _produtoService.Listar(nome, categoriaId, ativo, ehServico);
            return Ok(produtos);
        }

        [HttpGet("{id:int}")]
        public async Task<ActionResult<ProdutoRespostaDto>> ObterPorId(int id)
        {
            var produto = await _produtoService.ObterPorId(id);
            return Ok(produto);
        }

        [HttpPost]
        [Authorize(Roles = Perfis.Administrador)]
        public async Task<ActionResult<ProdutoRespostaDto>> Criar(ProdutoCriacaoDto dto)
        {
            var produto = await _produtoService.Criar(dto);
            return CreatedAtAction(nameof(ObterPorId), new { id = produto.Id }, produto);
        }

        [HttpPut("{id:int}")]
        [Authorize(Roles = Perfis.Administrador)]
        public async Task<ActionResult<ProdutoRespostaDto>> Atualizar(int id, ProdutoAtualizacaoDto dto)
        {
            var produto = await _produtoService.Atualizar(id, dto);
            return Ok(produto);
        }

        [HttpPatch("{id:int}/situacao")]
        [Authorize(Roles = Perfis.Administrador)]
        public async Task<ActionResult<ProdutoRespostaDto>> AlterarSituacao(int id, AtualizarSituacaoDto dto)
        {
            var produto = await _produtoService.AlterarSituacao(id, dto.Ativo);
            return Ok(produto);
        }
    }
}
