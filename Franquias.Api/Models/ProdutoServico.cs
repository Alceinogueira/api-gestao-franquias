namespace Franquias.Api.Models
{
    public class ProdutoServico
    {
        public int Id { get; set; }
        public string Nome { get; set; }
        public string Descricao { get; set; }
        public decimal PrecoBase { get; set; }
        public bool EhServico { get; set; }
        public bool Ativo { get; set; } = true;

        public int CategoriaId { get; set; }
        public Categoria Categoria { get; set; }

        public int? FornecedorId { get; set; }
        public Fornecedor Fornecedor { get; set; }
    }
}
