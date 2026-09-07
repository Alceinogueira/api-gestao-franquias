using System.Threading.Tasks;
using Franquias.Api.Common;
using Franquias.Api.DTOs;
using Franquias.Api.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Franquias.Api.Controllers
{
    [ApiController]
    [Route("api/categorias")]
    [Authorize]
    public class CategoriasController : ApiControllerBase
    {
        private readonly ICategoriaService _categoriaService;

        public CategoriasController(ICategoriaService categoriaService)
        {
            _categoriaService = categoriaService;
        }

        [HttpGet]
        public async Task<ActionResult> Listar()
        {
            var categorias = await _categoriaService.Listar();
            return Ok(categorias);
        }

        [HttpGet("{id:int}")]
        public async Task<ActionResult<CategoriaRespostaDto>> ObterPorId(int id)
        {
            var categoria = await _categoriaService.ObterPorId(id);
            return Ok(categoria);
        }

        [HttpPost]
        [Authorize(Roles = Perfis.Administrador)]
        public async Task<ActionResult<CategoriaRespostaDto>> Criar(CategoriaCriacaoDto dto)
        {
            var categoria = await _categoriaService.Criar(dto);
            return CreatedAtAction(nameof(ObterPorId), new { id = categoria.Id }, categoria);
        }

        [HttpPatch("{id:int}/situacao")]
        [Authorize(Roles = Perfis.Administrador)]
        public async Task<ActionResult<CategoriaRespostaDto>> AlterarSituacao(int id, AtualizarSituacaoDto dto)
        {
            var categoria = await _categoriaService.AlterarSituacao(id, dto.Ativo);
            return Ok(categoria);
        }
    }
}
