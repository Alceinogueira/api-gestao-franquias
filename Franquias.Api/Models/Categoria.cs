using System.Collections.Generic;

namespace Franquias.Api.Models
{
    public class Categoria
    {
        public int Id { get; set; }
        public string Nome { get; set; }
        public bool Ativa { get; set; } = true;

        public ICollection<ProdutoServico> Produtos { get; set; } = new List<ProdutoServico>();
    }
}
