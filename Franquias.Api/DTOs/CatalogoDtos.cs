using System.ComponentModel.DataAnnotations;

namespace Franquias.Api.DTOs
{
    public class CategoriaCriacaoDto
    {
        [Required]
        [MaxLength(80)]
        public string Nome { get; set; }
    }

    public class CategoriaRespostaDto
    {
        public int Id { get; set; }
        public string Nome { get; set; }
        public bool Ativa { get; set; }
    }

    public class ProdutoCriacaoDto
    {
        [Required]
        [MaxLength(120)]
        public string Nome { get; set; }

        public string Descricao { get; set; }

        [Range(0.01, 1000000)]
        public decimal PrecoBase { get; set; }

        public bool EhServico { get; set; }

        [Required]
        public int CategoriaId { get; set; }

        public int? FornecedorId { get; set; }
    }

    public class ProdutoAtualizacaoDto
    {
        [Required]
        [MaxLength(120)]
        public string Nome { get; set; }

        public string Descricao { get; set; }

        [Range(0.01, 1000000)]
        public decimal PrecoBase { get; set; }

        public bool EhServico { get; set; }

        [Required]
        public int CategoriaId { get; set; }

        public int? FornecedorId { get; set; }
    }

    public class ProdutoRespostaDto
    {
        public int Id { get; set; }
        public string Nome { get; set; }
        public string Descricao { get; set; }
        public decimal PrecoBase { get; set; }
        public bool EhServico { get; set; }
        public bool Ativo { get; set; }
        public int CategoriaId { get; set; }
        public string Categoria { get; set; }
        public int? FornecedorId { get; set; }
        public string Fornecedor { get; set; }
    }

    public class FornecedorCriacaoDto
    {
        [Required]
        [MaxLength(120)]
        public string Nome { get; set; }

        [Required]
        [MaxLength(20)]
        public string Cnpj { get; set; }

        [EmailAddress]
        public string Email { get; set; }

        public string Telefone { get; set; }
    }

    public class FornecedorAtualizacaoDto
    {
        [Required]
        [MaxLength(120)]
        public string Nome { get; set; }

        [Required]
        [MaxLength(20)]
        public string Cnpj { get; set; }

        [EmailAddress]
        public string Email { get; set; }

        public string Telefone { get; set; }
    }

    public class FornecedorRespostaDto
    {
        public int Id { get; set; }
        public string Nome { get; set; }
        public string Cnpj { get; set; }
        public string Email { get; set; }
        public string Telefone { get; set; }
        public bool Ativo { get; set; }
    }
}
