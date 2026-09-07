using System.Threading.Tasks;
using Franquias.Api.Common;
using Franquias.Api.DTOs;
using Franquias.Api.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Franquias.Api.Controllers
{
    [ApiController]
    [Route("api/fornecedores")]
    [Authorize]
    public class FornecedoresController : ApiControllerBase
    {
        private readonly IFornecedorService _fornecedorService;

        public FornecedoresController(IFornecedorService fornecedorService)
        {
            _fornecedorService = fornecedorService;
        }

        [HttpGet]
        public async Task<ActionResult> Listar([FromQuery] string texto, [FromQuery] bool? ativo)
        {
            var fornecedores = await _fornecedorService.Listar(texto, ativo);
            return Ok(fornecedores);
        }

        [HttpGet("{id:int}")]
        public async Task<ActionResult<FornecedorRespostaDto>> ObterPorId(int id)
        {
            var fornecedor = await _fornecedorService.ObterPorId(id);
            return Ok(fornecedor);
        }

        [HttpPost]
        [Authorize(Roles = Perfis.Administrador)]
        public async Task<ActionResult<FornecedorRespostaDto>> Criar(FornecedorCriacaoDto dto)
        {
            var fornecedor = await _fornecedorService.Criar(dto);
            return CreatedAtAction(nameof(ObterPorId), new { id = fornecedor.Id }, fornecedor);
        }

        [HttpPut("{id:int}")]
        [Authorize(Roles = Perfis.Administrador)]
        public async Task<ActionResult<FornecedorRespostaDto>> Atualizar(int id, FornecedorAtualizacaoDto dto)
        {
            var fornecedor = await _fornecedorService.Atualizar(id, dto);
            return Ok(fornecedor);
        }

        [HttpPatch("{id:int}/situacao")]
        [Authorize(Roles = Perfis.Administrador)]
        public async Task<ActionResult<FornecedorRespostaDto>> AlterarSituacao(int id, AtualizarSituacaoDto dto)
        {
            var fornecedor = await _fornecedorService.AlterarSituacao(id, dto.Ativo);
            return Ok(fornecedor);
        }
    }
}
