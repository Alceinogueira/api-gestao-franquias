using System;

namespace Franquias.Api.Models
{
    public class MovimentacaoEstoque
    {
        public int Id { get; set; }
        public TipoMovimentacao Tipo { get; set; }
        public int Quantidade { get; set; }
        public string Observacao { get; set; }
        public DateTime Data { get; set; } = DateTime.UtcNow;

        public int EstoqueId { get; set; }
        public Estoque Estoque { get; set; }
    }
}
