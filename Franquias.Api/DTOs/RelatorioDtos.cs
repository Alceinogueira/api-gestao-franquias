namespace Franquias.Api.DTOs
{
    public class FaturamentoUnidadeDto
    {
        public int UnidadeFranqueadaId { get; set; }
        public string Unidade { get; set; }
        public int QuantidadeVendas { get; set; }
        public decimal Faturamento { get; set; }
    }

    public class RankingUnidadeDto
    {
        public int Posicao { get; set; }
        public int UnidadeFranqueadaId { get; set; }
        public string Unidade { get; set; }
        public decimal Faturamento { get; set; }
    }

    public class ProdutoMaisVendidoDto
    {
        public int ProdutoServicoId { get; set; }
        public string Produto { get; set; }
        public int QuantidadeVendida { get; set; }
        public decimal TotalVendido { get; set; }
    }

    public class TotalRoyaltiesDto
    {
        public int? Ano { get; set; }
        public int? Mes { get; set; }
        public decimal TotalGerado { get; set; }
        public decimal TotalPago { get; set; }
        public decimal TotalPendente { get; set; }
    }

    public class ChamadosPorStatusDto
    {
        public string Status { get; set; }
        public int Quantidade { get; set; }
    }
}
