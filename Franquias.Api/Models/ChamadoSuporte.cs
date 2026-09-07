using System;

namespace Franquias.Api.Models
{
    public class ChamadoSuporte
    {
        public int Id { get; set; }
        public string Titulo { get; set; }
        public string Categoria { get; set; }
        public string Descricao { get; set; }
        public PrioridadeChamado Prioridade { get; set; }
        public StatusChamado Status { get; set; } = StatusChamado.Aberto;
        public DateTime DataAbertura { get; set; } = DateTime.UtcNow;
        public DateTime? DataEncerramento { get; set; }
        public string Resolucao { get; set; }

        public int UnidadeFranqueadaId { get; set; }
        public UnidadeFranqueada UnidadeFranqueada { get; set; }
    }
}
