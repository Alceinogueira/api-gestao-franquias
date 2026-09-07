using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Franquias.Api.DTOs
{
    public class ItemVendaCriacaoDto
    {
        [Required]
        public int ProdutoServicoId { get; set; }

        [Range(1, 1000000)]
        public int Quantidade { get; set; }
    }

    public class VendaCriacaoDto
    {
        [Required]
        public int UnidadeFranqueadaId { get; set; }

        [Required]
        [MinLength(1, ErrorMessage = "A venda deve possuir pelo menos um item.")]
        public List<ItemVendaCriacaoDto> Itens { get; set; } = new List<ItemVendaCriacaoDto>();
    }

    public class ItemVendaRespostaDto
    {
        public int ProdutoServicoId { get; set; }
        public string Produto { get; set; }
        public int Quantidade { get; set; }
        public decimal PrecoUnitario { get; set; }
        public decimal Subtotal { get; set; }
    }

    public class VendaRespostaDto
    {
        public int Id { get; set; }
        public int UnidadeFranqueadaId { get; set; }
        public string Unidade { get; set; }
        public DateTime Data { get; set; }
        public decimal ValorTotal { get; set; }
        public string Status { get; set; }
        public int UsuarioId { get; set; }
        public List<ItemVendaRespostaDto> Itens { get; set; } = new List<ItemVendaRespostaDto>();
    }
}
