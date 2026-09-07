using System;
using System.ComponentModel.DataAnnotations;
using Franquias.Api.Models;

namespace Franquias.Api.DTOs
{
    public class ChamadoCriacaoDto
    {
        [Required]
        public int UnidadeFranqueadaId { get; set; }

        [Required]
        [MaxLength(150)]
        public string Titulo { get; set; }

        [Required]
        [MaxLength(80)]
        public string Categoria { get; set; }

        [Required]
        public string Descricao { get; set; }

        [Required]
        public PrioridadeChamado Prioridade { get; set; }
    }

    public class ChamadoAtualizacaoDto
    {
        [Required]
        public StatusChamado Status { get; set; }

        public string Resolucao { get; set; }
    }

    public class ChamadoRespostaDto
    {
        public int Id { get; set; }
        public int UnidadeFranqueadaId { get; set; }
        public string Unidade { get; set; }
        public string Titulo { get; set; }
        public string Categoria { get; set; }
        public string Descricao { get; set; }
        public string Prioridade { get; set; }
        public string Status { get; set; }
        public DateTime DataAbertura { get; set; }
        public DateTime? DataEncerramento { get; set; }
        public string Resolucao { get; set; }
    }
}
