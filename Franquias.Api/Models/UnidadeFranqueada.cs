using System;
using System.Collections.Generic;

namespace Franquias.Api.Models
{
    public class UnidadeFranqueada
    {
        public int Id { get; set; }
        public string Nome { get; set; }
        public string Cnpj { get; set; }
        public string Cidade { get; set; }
        public string Estado { get; set; }
        public string Endereco { get; set; }
        public string Telefone { get; set; }
        public string Email { get; set; }
        public DateTime DataInicio { get; set; }
        public bool Ativa { get; set; } = true;
        public decimal PercentualRoyalty { get; set; }

        public int FranqueadoraId { get; set; }
        public Franqueadora Franqueadora { get; set; }

        public ICollection<Responsavel> Responsaveis { get; set; } = new List<Responsavel>();
        public ICollection<Estoque> Estoques { get; set; } = new List<Estoque>();
        public ICollection<Venda> Vendas { get; set; } = new List<Venda>();
        public ICollection<ChamadoSuporte> Chamados { get; set; } = new List<ChamadoSuporte>();
    }
}
