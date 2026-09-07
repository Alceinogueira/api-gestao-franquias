using System.Security.Claims;
using Franquias.Api.Common;
using Microsoft.AspNetCore.Mvc;

namespace Franquias.Api.Controllers
{
    public abstract class ApiControllerBase : ControllerBase
    {
        protected int UsuarioAtualId
        {
            get
            {
                var claim = User.FindFirst(ClaimTypes.NameIdentifier);
                return claim == null ? 0 : int.Parse(claim.Value);
            }
        }

        protected string PerfilAtual
        {
            get
            {
                var claim = User.FindFirst(ClaimTypes.Role);
                return claim == null ? null : claim.Value;
            }
        }

        protected int? UnidadeDoUsuario
        {
            get
            {
                var claim = User.FindFirst("unidadeId");
                if (claim == null || string.IsNullOrEmpty(claim.Value))
                {
                    return null;
                }

                return int.Parse(claim.Value);
            }
        }

        protected bool EhAdministrador
        {
            get { return PerfilAtual == Perfis.Administrador; }
        }
    }
}
