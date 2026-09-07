using System;
using System.ComponentModel.DataAnnotations;
using Franquias.Api.Models;

namespace Franquias.Api.DTOs
{
    public class EstoqueCriacaoDto
    {
        [Required]
        public int UnidadeFranqueadaId { get; set; }

        [Required]
        public int ProdutoServicoId { get; set; }

        [Range(0, 1000000)]
        public int QuantidadeInicial { get; set; }

        [Range(0, 1000000)]
        public int QuantidadeMinima { get; set; }
    }

    public class MovimentacaoEstoqueDto
    {
        [Required]
        public int EstoqueId { get; set; }

        [Required]
        public TipoMovimentacao Tipo { get; set; }

        [Range(1, 1000000)]
        public int Quantidade { get; set; }

        public string Observacao { get; set; }
    }

    public class MovimentacaoRespostaDto
    {
        public int Id { get; set; }
        public string Tipo { get; set; }
        public int Quantidade { get; set; }
        public string Observacao { get; set; }
        public DateTime Data { get; set; }
    }

    public class EstoqueRespostaDto
    {
        public int Id { get; set; }
        public int UnidadeFranqueadaId { get; set; }
        public string Unidade { get; set; }
        public int ProdutoServicoId { get; set; }
        public string Produto { get; set; }
        public int Quantidade { get; set; }
        public int QuantidadeMinima { get; set; }
        public bool AbaixoDoMinimo { get; set; }
    }
}
