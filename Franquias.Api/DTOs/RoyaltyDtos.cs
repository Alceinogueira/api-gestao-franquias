using System;
using System.ComponentModel.DataAnnotations;

namespace Franquias.Api.DTOs
{
    public class GerarRoyaltyDto
    {
        [Required]
        public int UnidadeFranqueadaId { get; set; }

        [Range(2000, 2100)]
        public int Ano { get; set; }

        [Range(1, 12)]
        public int Mes { get; set; }
    }

    public class RoyaltyRespostaDto
    {
        public int Id { get; set; }
        public int UnidadeFranqueadaId { get; set; }
        public string Unidade { get; set; }
        public int Ano { get; set; }
        public int Mes { get; set; }
        public decimal FaturamentoPeriodo { get; set; }
        public decimal PercentualAplicado { get; set; }
        public decimal ValorDevido { get; set; }
        public string StatusPagamento { get; set; }
        public DateTime DataGeracao { get; set; }
        public DateTime? DataPagamento { get; set; }
    }
}
