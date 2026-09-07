using System;
using System.Collections.Generic;

namespace Franquias.Api.Models
{
    public class Franqueadora
    {
        public int Id { get; set; }
        public string RazaoSocial { get; set; }
        public string NomeFantasia { get; set; }
        public string Cnpj { get; set; }
        public string Email { get; set; }
        public string Telefone { get; set; }
        public decimal PercentualRoyaltyPadrao { get; set; }
        public DateTime DataCadastro { get; set; } = DateTime.UtcNow;

        public ICollection<UnidadeFranqueada> Unidades { get; set; } = new List<UnidadeFranqueada>();
    }
}
