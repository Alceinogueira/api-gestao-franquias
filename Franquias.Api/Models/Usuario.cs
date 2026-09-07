using System;

namespace Franquias.Api.Models
{
    public class Usuario
    {
        public int Id { get; set; }
        public string Nome { get; set; }
        public string Email { get; set; }
        public string SenhaHash { get; set; }
        public Perfil Perfil { get; set; }
        public bool Ativo { get; set; } = true;
        public DateTime DataCadastro { get; set; } = DateTime.UtcNow;

        public int? UnidadeFranqueadaId { get; set; }
        public UnidadeFranqueada UnidadeFranqueada { get; set; }
    }
}
