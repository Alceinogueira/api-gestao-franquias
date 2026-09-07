using System.Threading.Tasks;
using Franquias.Api.Common;
using Franquias.Api.DTOs;
using Franquias.Api.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Franquias.Api.Controllers
{
    [ApiController]
    [Route("api/auth")]
    [Authorize]
    public class AuthController : ApiControllerBase
    {
        private readonly IUsuarioService _usuarioService;

        public AuthController(IUsuarioService usuarioService)
        {
            _usuarioService = usuarioService;
        }

        [HttpPost("login")]
        [AllowAnonymous]
        public async Task<ActionResult<LoginRespostaDto>> Login(LoginDto dto)
        {
            var resposta = await _usuarioService.Login(dto);
            return Ok(resposta);
        }

        [HttpPost("usuarios")]
        [Authorize(Roles = Perfis.Administrador)]
        public async Task<ActionResult<UsuarioRespostaDto>> Registrar(RegistroUsuarioDto dto)
        {
            var usuario = await _usuarioService.Registrar(dto);
            return Created("/api/auth/usuarios/" + usuario.Id, usuario);
        }

        [HttpGet("usuarios")]
        [Authorize(Roles = Perfis.Administrador)]
        public async Task<ActionResult> Listar()
        {
            var usuarios = await _usuarioService.Listar();
            return Ok(usuarios);
        }

        [HttpPatch("usuarios/{id:int}/situacao")]
        [Authorize(Roles = Perfis.Administrador)]
        public async Task<ActionResult<UsuarioRespostaDto>> AlterarSituacao(int id, AtualizarSituacaoDto dto)
        {
            var usuario = await _usuarioService.AlterarSituacao(id, dto.Ativo);
            return Ok(usuario);
        }
    }
}
