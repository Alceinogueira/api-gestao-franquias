using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Franquias.Api.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace Franquias.Api.Services
{
    public interface ITokenService
    {
        int MinutosExpiracao { get; }
        string GerarToken(Usuario usuario);
    }

    public class TokenService : ITokenService
    {
        private readonly IConfiguration _configuracao;

        public TokenService(IConfiguration configuracao)
        {
            _configuracao = configuracao;
        }

        public int MinutosExpiracao
        {
            get { return int.Parse(_configuracao["Jwt:MinutosExpiracao"]); }
        }

        public string GerarToken(Usuario usuario)
        {
            var secao = _configuracao.GetSection("Jwt");
            var chave = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secao["Chave"]));
            var credenciais = new SigningCredentials(chave, SecurityAlgorithms.HmacSha256);

            var unidade = usuario.UnidadeFranqueadaId.HasValue
                ? usuario.UnidadeFranqueadaId.Value.ToString()
                : string.Empty;

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, usuario.Id.ToString()),
                new Claim(ClaimTypes.Name, usuario.Nome),
                new Claim(ClaimTypes.Role, usuario.Perfil.ToString()),
                new Claim("unidadeId", unidade)
            };

            var token = new JwtSecurityToken(
                issuer: secao["Emissor"],
                audience: secao["Audiencia"],
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(MinutosExpiracao),
                signingCredentials: credenciais);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
