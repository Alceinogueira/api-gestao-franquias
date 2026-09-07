using System;

namespace Franquias.Api.Models
{
    public class Royalty
    {
        public int Id { get; set; }
        public int Ano { get; set; }
        public int Mes { get; set; }
        public decimal FaturamentoPeriodo { get; set; }
        public decimal PercentualAplicado { get; set; }
        public decimal ValorDevido { get; set; }
        public StatusPagamento StatusPagamento { get; set; } = StatusPagamento.Pendente;
        public DateTime DataGeracao { get; set; } = DateTime.UtcNow;
        public DateTime? DataPagamento { get; set; }

        public int UnidadeFranqueadaId { get; set; }
        public UnidadeFranqueada UnidadeFranqueada { get; set; }
    }
}
