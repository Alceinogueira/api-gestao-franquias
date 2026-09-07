using System.Collections.Generic;

namespace Franquias.Api.Models
{
    public class Fornecedor
    {
        public int Id { get; set; }
        public string Nome { get; set; }
        public string Cnpj { get; set; }
        public string Email { get; set; }
        public string Telefone { get; set; }
        public bool Ativo { get; set; } = true;

        public ICollection<ProdutoServico> Produtos { get; set; } = new List<ProdutoServico>();
    }
}
