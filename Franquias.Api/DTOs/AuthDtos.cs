using System;
using System.ComponentModel.DataAnnotations;
using Franquias.Api.Models;

namespace Franquias.Api.DTOs
{
    public class LoginDto
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        public string Senha { get; set; }
    }

    public class LoginRespostaDto
    {
        public string Token { get; set; }
        public string Nome { get; set; }
        public string Perfil { get; set; }
        public DateTime ExpiraEm { get; set; }
    }

    public class RegistroUsuarioDto
    {
        [Required]
        [MaxLength(120)]
        public string Nome { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        [MinLength(6)]
        public string Senha { get; set; }

        [Required]
        public Perfil Perfil { get; set; }

        public int? UnidadeFranqueadaId { get; set; }
    }

    public class UsuarioRespostaDto
    {
        public int Id { get; set; }
        public string Nome { get; set; }
        public string Email { get; set; }
        public string Perfil { get; set; }
        public bool Ativo { get; set; }
        public int? UnidadeFranqueadaId { get; set; }
    }

    public class AtualizarSituacaoDto
    {
        [Required]
        public bool Ativo { get; set; }
    }
}
