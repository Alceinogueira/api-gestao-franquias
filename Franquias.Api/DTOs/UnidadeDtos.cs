using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Franquias.Api.DTOs
{
    public class ResponsavelDto
    {
        [Required]
        [MaxLength(120)]
        public string Nome { get; set; }

        [Required]
        [MaxLength(20)]
        public string Cpf { get; set; }

        [EmailAddress]
        public string Email { get; set; }

        public string Telefone { get; set; }

        public bool Franqueado { get; set; }
    }

    public class UnidadeCriacaoDto
    {
        [Required]
        [MaxLength(120)]
        public string Nome { get; set; }

        [Required]
        [MaxLength(20)]
        public string Cnpj { get; set; }

        [Required]
        public string Cidade { get; set; }

        [Required]
        [MaxLength(2)]
        public string Estado { get; set; }

        public string Endereco { get; set; }
        public string Telefone { get; set; }

        [EmailAddress]
        public string Email { get; set; }

        [Required]
        public DateTime DataInicio { get; set; }

        [Range(0, 100)]
        public decimal PercentualRoyalty { get; set; }

        [Required]
        public int FranqueadoraId { get; set; }

        public List<ResponsavelDto> Responsaveis { get; set; } = new List<ResponsavelDto>();
    }

    public class UnidadeAtualizacaoDto
    {
        [Required]
        [MaxLength(120)]
        public string Nome { get; set; }

        [Required]
        [MaxLength(20)]
        public string Cnpj { get; set; }

        [Required]
        public string Cidade { get; set; }

        [Required]
        [MaxLength(2)]
        public string Estado { get; set; }

        public string Endereco { get; set; }
        public string Telefone { get; set; }

        [EmailAddress]
        public string Email { get; set; }

        [Required]
        public DateTime DataInicio { get; set; }

        [Range(0, 100)]
        public decimal PercentualRoyalty { get; set; }
    }

    public class ResponsavelRespostaDto
    {
        public int Id { get; set; }
        public string Nome { get; set; }
        public string Cpf { get; set; }
        public string Email { get; set; }
        public string Telefone { get; set; }
        public bool Franqueado { get; set; }
    }

    public class UnidadeRespostaDto
    {
        public int Id { get; set; }
        public string Nome { get; set; }
        public string Cnpj { get; set; }
        public string Cidade { get; set; }
        public string Estado { get; set; }
        public string Endereco { get; set; }
        public string Telefone { get; set; }
        public string Email { get; set; }
        public DateTime DataInicio { get; set; }
        public bool Ativa { get; set; }
        public decimal PercentualRoyalty { get; set; }
        public int FranqueadoraId { get; set; }
        public List<ResponsavelRespostaDto> Responsaveis { get; set; } = new List<ResponsavelRespostaDto>();
    }
}
